using System;
using System.Dynamic;
using System.Windows.Markup;

namespace DO;

/// <summary>
/// Task entity
/// </summary>

public record Task
{
	int _id { get; set; }
	string _alias { get; set; }
	string _description { get; set; }
	DateTime _createdAtDate { get; set; }
	TimeSpan _requiredEffortTime { get; set; }
	bool _isMilestone { get; set; }
	EngineerExperience _complexity { get; set; }
	DateTime _startDate { get; set; }
	DateTime _scheduledDate { get; set; }
	DateTime _deadlineDate {  get; set; }
	DateTime _completeDate { get; set; }
	string _deliverables { get; set; }
	string _remarks { get; set; }
	int _engineerId { get; set; }

	
}
