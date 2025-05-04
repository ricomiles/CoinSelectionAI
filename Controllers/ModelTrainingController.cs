using CardanoCoinSelection.Services;
using Microsoft.AspNetCore.Mvc;
namespace CardanoCoinSelection.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ModelTrainingController : ControllerBase
{
    private readonly ModelTrainingService _trainingService;
    private readonly ILogger<ModelTrainingController> _logger;

    public ModelTrainingController(
        ModelTrainingService trainingService,
        ILogger<ModelTrainingController> logger)
    {
        _trainingService = trainingService;
        _logger = logger;
    }

    [HttpPost("train")]
    public async Task<IActionResult> TrainModel([FromBody] TrainingConfiguration config)
    {
        _logger.LogInformation("Received training request for {ConfigName}", config.ConfigurationName);

        var metrics = await _trainingService.TrainModelAsync(config);

        return Ok(new
        {
            modelVersion = metrics.ModelVersion,
            status = string.IsNullOrEmpty(metrics.ErrorMessage) ? "completed" : "failed",
            trainingDuration = metrics.TrainingDuration.TotalSeconds,
            accuracy = metrics.Accuracy,
            metrics = new
            {
                validation = metrics.ValidationResults,
                featureImportance = metrics.FeatureImportance
            }
        });
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetModelStatus()
    {
        var modelArtifacts = await _trainingService.LoadLatestModelArtifactsAsync();

        if (modelArtifacts == null)
        {
            return NotFound(new { message = "No trained model found" });
        }

        return Ok(new
        {
            modelVersion = modelArtifacts.ModelVersion,
            lastUpdated = modelArtifacts.LastUpdated,
            metrics = modelArtifacts.Metrics,
            configuration = new
            {
                epochs = modelArtifacts.TrainingConfig.Epochs,
                datasetSize = modelArtifacts.TrainingConfig.DatasetSize,
                hyperparameters = modelArtifacts.Hyperparameters
            }
        });
    }

}