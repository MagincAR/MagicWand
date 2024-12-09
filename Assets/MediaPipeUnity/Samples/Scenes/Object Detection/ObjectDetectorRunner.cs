using System;
using System.Collections;
using Mediapipe.Tasks.Vision.ObjectDetector;
using UnityEngine;
using UnityEngine.Rendering;
using ObjectDetectionResult = Mediapipe.Tasks.Components.Containers.DetectionResult;

namespace Mediapipe.Unity.Sample.ObjectDetection
{
    public class ObjectDetectorRunner : VisionTaskApiRunner<ObjectDetector>
    {
        [SerializeField] private DetectionResultAnnotationController _detectionResultAnnotationController;
        [SerializeField] private GameObject fireballPrefab; // Fireball prefab�� �߰��մϴ�.

        private Experimental.TextureFramePool _textureFramePool;

        public readonly ObjectDetectionConfig config = new ObjectDetectionConfig();

        public override void Stop()
        {
            base.Stop();
            _textureFramePool?.Dispose();
            _textureFramePool = null;
        }

        protected override IEnumerator Run()
        {
            Debug.Log($"Delegate = {config.Delegate}");
            Debug.Log($"Model = {config.ModelName}");
            Debug.Log($"Running Mode = {config.RunningMode}");
            Debug.Log($"Score Threshold = {config.ScoreThreshold}");
            Debug.Log($"Max Results = {config.MaxResults}");

            yield return AssetLoader.PrepareAssetAsync(config.ModelPath);

            var options = config.GetObjectDetectorOptions(config.RunningMode == Tasks.Vision.Core.RunningMode.LIVE_STREAM ? OnObjectDetectionsOutput : null);
            taskApi = ObjectDetector.CreateFromOptions(options, GpuManager.GpuResources);
            var imageSource = ImageSourceProvider.ImageSource;

            yield return imageSource.Play();

            if (!imageSource.isPrepared)
            {
                Debug.LogError("Failed to start ImageSource, exiting...");
                yield break;
            }

            _textureFramePool = new Experimental.TextureFramePool(imageSource.textureWidth, imageSource.textureHeight, TextureFormat.RGBA32, 10);
            screen.Initialize(imageSource);
            SetupAnnotationController(_detectionResultAnnotationController, imageSource);

            var transformationOptions = imageSource.GetTransformationOptions();
            var flipHorizontally = transformationOptions.flipHorizontally;
            var flipVertically = transformationOptions.flipVertically;
            var imageProcessingOptions = new Tasks.Vision.Core.ImageProcessingOptions(rotationDegrees: (int)transformationOptions.rotationAngle);

            AsyncGPUReadbackRequest req = default;
            var waitUntilReqDone = new WaitUntil(() => req.done);
            var result = ObjectDetectionResult.Alloc(Math.Max(options.maxResults ?? 0, 0));

            var canUseGpuImage = options.baseOptions.delegateCase == Tasks.Core.BaseOptions.Delegate.GPU &&
              SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3 &&
              GpuManager.GpuResources != null;
            using var glContext = canUseGpuImage ? GpuManager.GetGlContext() : null;

            while (true)
            {
                if (isPaused)
                {
                    yield return new WaitWhile(() => isPaused);
                }

                if (!_textureFramePool.TryGetTextureFrame(out var textureFrame))
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }

                Image image;
                if (canUseGpuImage)
                {
                    yield return new WaitForEndOfFrame();
                    textureFrame.ReadTextureOnGPU(imageSource.GetCurrentTexture(), flipHorizontally, flipVertically);
                    image = textureFrame.BuildGpuImage(glContext);
                }
                else
                {
                    req = textureFrame.ReadTextureAsync(imageSource.GetCurrentTexture(), flipHorizontally, flipVertically);
                    yield return waitUntilReqDone;

                    if (req.hasError)
                    {
                        Debug.LogError($"Failed to read texture from the image source, exiting...");
                        break;
                    }
                    image = textureFrame.BuildCPUImage();
                    textureFrame.Release();
                }

                switch (taskApi.runningMode)
                {
                    case Tasks.Vision.Core.RunningMode.IMAGE:
                        if (taskApi.TryDetect(image, imageProcessingOptions, ref result))
                        {
                            _detectionResultAnnotationController.DrawNow(result);
                            SpawnFireballsFromDetectionResult(result);  // Fireball ����
                        }
                        else
                        {
                            _detectionResultAnnotationController.DrawNow(default);
                        }
                        break;
                    case Tasks.Vision.Core.RunningMode.VIDEO:
                        if (taskApi.TryDetectForVideo(image, GetCurrentTimestampMillisec(), imageProcessingOptions, ref result))
                        {
                            _detectionResultAnnotationController.DrawNow(result);
                            SpawnFireballsFromDetectionResult(result);  // Fireball ����
                        }
                        else
                        {
                            _detectionResultAnnotationController.DrawNow(default);
                        }
                        break;
                    case Tasks.Vision.Core.RunningMode.LIVE_STREAM:
                        taskApi.DetectAsync(image, GetCurrentTimestampMillisec(), imageProcessingOptions);
                        break;
                }
            }
        }

        private void OnObjectDetectionsOutput(ObjectDetectionResult result, Image image, long timestamp)
        {
            // Draw the detected objects
            _detectionResultAnnotationController.DrawLater(result);

            // Fireball ����
            SpawnFireballsFromDetectionResult(result);
        }

        // ��ü�� Fireball�� �߰��ϴ� �޼���
        private void SpawnFireballsFromDetectionResult(ObjectDetectionResult result)
        {
            foreach (var detection in result.detections)
            {
                // boundingBox�� ��ǥ�� ������� ��ü�� �߾� ��ġ ���
                float centerX = (detection.boundingBox.left + detection.boundingBox.right) / 2f;
                float centerY = (detection.boundingBox.top + detection.boundingBox.bottom) / 2f;

                // ��ġ�� ���÷� ȭ���� �߾ӿ��� ���� �����ϵ��� ���
                Vector3 spawnPosition = new Vector3(centerX, centerY, Camera.main.nearClipPlane); // Z ���� ī�޶��� nearClipPlane�� �°� ����

                // ȭ�� ��ǥ�� ���� ��ǥ�� ��ȯ
                spawnPosition = Camera.main.ScreenToWorldPoint(new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z));

                // �� ����
                SpawnFireballAtPosition(spawnPosition);
            }
        }

        private void SpawnFireballAtPosition(Vector3 position)
        {
            if (fireballPrefab != null)
            {
                // Fireball�� �ش� ��ġ�� �ν��Ͻ�ȭ
                Instantiate(fireballPrefab, position, Quaternion.identity);
            }
            else
            {
                Debug.LogError("Fireball prefab is not assigned!");
            }
        }
    }
}
