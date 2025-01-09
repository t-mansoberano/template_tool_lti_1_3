﻿namespace gec.Application.Features.Instructors.Evaluations.Queries.GetCompleteEvaluationsView.Models;

public class EvaluationResult
{
    public string Id { get; set; } = ""; // Matches the Id of an EvaluationStructureDTO
    public string AchievementLevel { get; set; } = "";
    public string Comments { get; set; } = "";
    public bool IsEvaluated { get; set; }
}