namespace DO;

/// <summary>
/// Dependency record (PDS)
/// </summary>
/// <param name="id"></param>
/// <param name="depenentTask"></param>
/// <param name="dependsOnTask"></param>
public record Dependency
(
	int id,
	int dependentTask,
	int dependsOnTask,
	String? customerEmail = null,
	String? shippingAddress = null,
	DateTime? orderCreationDate = null,
	DateTime? shippingDate = null,
	DateTime? deliveryDate = null
);