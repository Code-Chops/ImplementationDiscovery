namespace CodeChops.ImplementationDiscovery.UnitTests.ImplementedRecords;

[DiscoverImplementations]
public abstract partial record AbstractRecordToImplement(int A);

public partial record RecordImplementationMock(int A) : AbstractRecordToImplement(A);