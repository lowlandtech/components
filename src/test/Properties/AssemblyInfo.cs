using Xunit;

// Several acceptance scenarios pack this repository's projects into a private feed
// and build the same projects. The packing tool serializes its own build and pack,
// but the packaging scenarios of CPKG0001 and CPKG0002 pack through their own script,
// so two of them can still build src/lib or src/blocks at the same time and fail with
// a compiler error about a file the other process holds. The assembly therefore runs
// one test collection at a time: the suite is slower, and the packing acceptance is
// deterministic instead of racing on a shared intermediate output.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
