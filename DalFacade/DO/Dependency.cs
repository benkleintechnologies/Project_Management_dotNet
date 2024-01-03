namespace DO;

/// <summary>
/// Dependency Entity (PDS)
/// </summary>
/// <param name="id"></param>
/// <param name="dependentTask"></param>
/// <param name="dependsOnTask"></param>
/// <param name="customerEmail"></param>
/// <param name="shippingAddress"></param>
/// <param name="orderCreationDate"></param>
/// <param name="shippingDate"></param>
/// <param name="deliveryDate"></param>
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