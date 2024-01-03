using System;
using System.Dynamic;
using System.Windows.Markup;

namespace DO;

/// <summary>
/// Task entity
/// </summary>

public record Task
	(
		int id,
		string nickname,
		string description,
		bool isMilestone,
		DateTime dateCreated,
		DateTime projectedStartDate,
		DateTime actualStartDate,
		DateTime duration,
		DateTime deadline,
		DateTime actualEndDate,
		string deliverable, //not sure what should be the type
		string notes,
		int assignedEngineerId,
		Difficulty degreeOfDifficulty
	);
