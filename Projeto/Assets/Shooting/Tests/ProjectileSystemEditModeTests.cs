using NUnit.Framework;
using Unity.Collections;
using Unity.Jobs;

[TestFixture]
public class ProjectileSystemEditModeTests
{
    private NativeStream nativeStream;

    [SetUp]
    public void Setup()
    {
        nativeStream = new NativeStream(1, Allocator.Persistent);
    }

    [TearDown]
    public void Teardown()
    {
        nativeStream.Dispose();
    }

    [Test]
    public void TestJobCompletion()
    {
        var job = new ConstructJob { Container = nativeStream.AsWriter() };
        JobHandle jobHandle = job.Schedule();

        jobHandle.Complete();

        // Add assertions to verify the expected state of the NativeStream after job completion
    }

    private struct ConstructJob : IJob
    {
        public NativeStream.Writer Container;

        public void Execute()
        {
            // Job logic to write to the NativeStream
        }
    }
}