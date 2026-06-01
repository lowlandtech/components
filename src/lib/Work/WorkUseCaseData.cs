namespace LowlandTech.Components.Work;

public static class WorkUseCaseData
{
    public static readonly DateTimeOffset Now = new(2026, 06, 01, 12, 00, 00, TimeSpan.Zero);

    public static WorkAppState HealthyCluster()
    {
        var jobs = BuildJobs();
        var queues = BuildQueues(jobs);
        var servers = BuildServers();
        var recurring = BuildRecurringJobs();
        var metrics = BuildMetrics(jobs);
        var statistics = BuildStatistics(jobs, queues, servers, recurring);

        return new WorkAppState
        {
            Ui = WorkUiDefaults.Empty,
            Jobs = jobs,
            Queues = queues,
            Servers = servers,
            RecurringJobs = recurring,
            Metrics = metrics,
            Statistics = statistics,
            GraphData = BuildGraphData()
        };
    }

    public static IReadOnlyList<WorkItem> BuildJobs()
    {
        var jobs = new List<WorkItem>();
        int id = 1;

        // 15 succeeded jobs
        jobs.AddRange([
            new($"job-{id++:D3}", "Orders.ProcessPayment #1",  WorkItemState.Succeeded, Now.AddMinutes(-5),   Now.AddMinutes(-4.9), null, Now.AddMinutes(-4.8), Now.AddMinutes(-2),  null, null, null, "worker-01", "critical",  null, null, null, null, 120, 80,  Parameters: new Dictionary<string,string>{["orderId"] = "ORD-1001"}),
            new($"job-{id++:D3}", "Orders.ProcessPayment #2",  WorkItemState.Succeeded, Now.AddMinutes(-10),  Now.AddMinutes(-9.9), null, Now.AddMinutes(-9.8), Now.AddMinutes(-7),  null, null, null, "worker-01", "critical",  null, null, null, null, 110, 75,  Parameters: new Dictionary<string,string>{["orderId"] = "ORD-1002"}),
            new($"job-{id++:D3}", "Email.SendDigest",          WorkItemState.Succeeded, Now.AddMinutes(-15),  Now.AddMinutes(-14.9),null, Now.AddMinutes(-14.8),Now.AddMinutes(-12), null, null, null, "worker-02", "emails",    null, null, null, null, 200, 150, Parameters: new Dictionary<string,string>{["recipient"] = "user@example.com"}),
            new($"job-{id++:D3}", "Reports.GenerateDaily #1",  WorkItemState.Succeeded, Now.AddMinutes(-20),  Now.AddMinutes(-19.9),null, Now.AddMinutes(-19.8),Now.AddMinutes(-17), null, null, null, "worker-03", "imports",   null, null, null, null, 450, 300, Parameters: new Dictionary<string,string>{["date"] = "2026-05-31"}),
            new($"job-{id++:D3}", "Inventory.UpdateStock",     WorkItemState.Succeeded, Now.AddMinutes(-25),  Now.AddMinutes(-24.9),null, Now.AddMinutes(-24.8),Now.AddMinutes(-22), null, null, null, "worker-01", "default",   null, null, null, null, 95,  60),
            new($"job-{id++:D3}", "Agents.RunTask #1",         WorkItemState.Succeeded, Now.AddMinutes(-30),  Now.AddMinutes(-29.9),null, Now.AddMinutes(-29.8),Now.AddMinutes(-27), null, null, null, "worker-02", "agents",    null, null, null, null, 300, 200),
            new($"job-{id++:D3}", "Orders.ProcessPayment #3",  WorkItemState.Succeeded, Now.AddMinutes(-35),  Now.AddMinutes(-34.9),null, Now.AddMinutes(-34.8),Now.AddMinutes(-32), null, null, null, "worker-01", "critical",  null, null, null, null, 115, 78),
            new($"job-{id++:D3}", "Email.SendReceipt",         WorkItemState.Succeeded, Now.AddMinutes(-40),  Now.AddMinutes(-39.9),null, Now.AddMinutes(-39.8),Now.AddMinutes(-37), null, null, null, "worker-02", "emails",    null, null, null, null, 180, 120),
            new($"job-{id++:D3}", "Reports.GenerateWeekly",    WorkItemState.Succeeded, Now.AddMinutes(-45),  Now.AddMinutes(-44.9),null, Now.AddMinutes(-44.8),Now.AddMinutes(-42), null, null, null, "worker-03", "imports",   null, null, null, null, 600, 400),
            new($"job-{id++:D3}", "Maintenance.Cleanup #1",    WorkItemState.Succeeded, Now.AddMinutes(-50),  Now.AddMinutes(-49.9),null, Now.AddMinutes(-49.8),Now.AddMinutes(-47), null, null, null, "worker-01", "default",   null, null, null, null, 250, 180),
            new($"job-{id++:D3}", "Orders.IssueRefund",        WorkItemState.Succeeded, Now.AddMinutes(-55),  Now.AddMinutes(-54.9),null, Now.AddMinutes(-54.8),Now.AddMinutes(-52), null, null, null, "worker-01", "critical",  null, null, null, null, 130, 90),
            new($"job-{id++:D3}", "Email.SendCampaign",        WorkItemState.Succeeded, Now.AddMinutes(-60),  Now.AddMinutes(-59.9),null, Now.AddMinutes(-59.8),Now.AddMinutes(-57), null, null, null, "worker-02", "emails",    null, null, null, null, 350, 250),
            new($"job-{id++:D3}", "Sync.ExternalData #3",      WorkItemState.Succeeded, Now.AddMinutes(-65),  Now.AddMinutes(-64.9),null, Now.AddMinutes(-64.8),Now.AddMinutes(-62), null, null, null, "worker-03", "imports",   null, null, null, null, 500, 350),
            new($"job-{id++:D3}", "Agents.RunTask #2",         WorkItemState.Succeeded, Now.AddMinutes(-70),  Now.AddMinutes(-69.9),null, Now.AddMinutes(-69.8),Now.AddMinutes(-67), null, null, null, "worker-02", "agents",    null, null, null, null, 280, 190),
            new($"job-{id++:D3}", "Notifications.SendDigest #1",WorkItemState.Succeeded, Now.AddMinutes(-75),  Now.AddMinutes(-74.9),null, Now.AddMinutes(-74.8),Now.AddMinutes(-72), null, null, null, "worker-01", "default",   null, null, null, null, 160, 110),
        ]);

        // 5 failed jobs
        jobs.AddRange([
            new($"job-{id++:D3}", "Orders.ProcessPayment #4",  WorkItemState.Failed,    Now.AddMinutes(-8),   Now.AddMinutes(-7.9), null, Now.AddMinutes(-7.8), null, Now.AddMinutes(-6),  null, null, "worker-01", "critical",  "Payment gateway timeout",       "System.TimeoutException",         "The request timed out after 30s",         null, 300, 200),
            new($"job-{id++:D3}", "Email.SendDigest #1",       WorkItemState.Failed,    Now.AddMinutes(-18),  Now.AddMinutes(-17.9),null, Now.AddMinutes(-17.8),null, Now.AddMinutes(-16), null, null, "worker-02", "emails",    "SMTP connection refused",       "System.Net.Sockets.SocketException", "Connection refused",                     null, 150, 100),
            new($"job-{id++:D3}", "Reports.GenerateDaily #2",  WorkItemState.Failed,    Now.AddMinutes(-28),  Now.AddMinutes(-27.9),null, Now.AddMinutes(-27.8),null, Now.AddMinutes(-26), null, null, "worker-03", "imports",   "Out of memory",                 "System.OutOfMemoryException",     "Insufficient memory to continue execution", null, 500, 350),
            new($"job-{id++:D3}", "Sync.ExternalData #1",      WorkItemState.Failed,    Now.AddMinutes(-48),  Now.AddMinutes(-47.9),null, Now.AddMinutes(-47.8),null, Now.AddMinutes(-46), null, null, "worker-03", "imports",   "API returned 503",              "System.HttpRequestException",     "Service Unavailable",                    null, 250, 180),
            new($"job-{id++:D3}", "Agents.RunTask #3",         WorkItemState.Failed,    Now.AddMinutes(-68),  Now.AddMinutes(-67.9),null, Now.AddMinutes(-67.8),null, Now.AddMinutes(-66), null, null, "worker-02", "agents",    "Invalid task configuration",    "System.ArgumentException",        "Task 'unknown-task' not found",          null, 100, 70),
        ]);

        // 3 processing jobs
        jobs.AddRange([
            new($"job-{id++:D3}", "Orders.ProcessPayment #5",  WorkItemState.Processing,Now.AddMinutes(-2),   Now.AddMinutes(-1.9), null, Now.AddMinutes(-1.8), null, null, null, null, "worker-01", "critical",  null, null, null, null, 45, 30),
            new($"job-{id++:D3}", "Email.SendCampaign #1",     WorkItemState.Processing,Now.AddMinutes(-4),   Now.AddMinutes(-3.9), null, Now.AddMinutes(-3.8), null, null, null, null, "worker-02", "emails",    null, null, null, null, 90, 60),
            new($"job-{id++:D3}", "Reports.GenerateDaily #3",  WorkItemState.Processing,Now.AddMinutes(-6),   Now.AddMinutes(-5.9), null, Now.AddMinutes(-5.8), null, null, null, null, "worker-03", "imports",   null, null, null, null, 120, 80),
        ]);

        // 8 enqueued jobs
        jobs.AddRange([
            new($"job-{id++:D3}", "Orders.ProcessPayment #6",  WorkItemState.Enqueued,  Now.AddMinutes(-3),   Now.AddMinutes(-2.9),  QueueName: "critical"),
            new($"job-{id++:D3}", "Orders.ProcessPayment #7",  WorkItemState.Enqueued,  Now.AddMinutes(-6),   Now.AddMinutes(-5.9),  QueueName: "critical"),
            new($"job-{id++:D3}", "Email.SendDigest #2",       WorkItemState.Enqueued,  Now.AddMinutes(-9),   Now.AddMinutes(-8.9),  QueueName: "emails"),
            new($"job-{id++:D3}", "Inventory.UpdateStock #1",  WorkItemState.Enqueued,  Now.AddMinutes(-12),  Now.AddMinutes(-11.9), QueueName: "default"),
            new($"job-{id++:D3}", "Maintenance.Cleanup #2",    WorkItemState.Enqueued,  Now.AddMinutes(-15),  Now.AddMinutes(-14.9), QueueName: "default"),
            new($"job-{id++:D3}", "Agents.RunTask #4",         WorkItemState.Enqueued,  Now.AddMinutes(-18),  Now.AddMinutes(-17.9), QueueName: "agents"),
            new($"job-{id++:D3}", "Notifications.SendDigest #2",WorkItemState.Enqueued,  Now.AddMinutes(-21),  Now.AddMinutes(-20.9), QueueName: "default"),
            new($"job-{id++:D3}", "Orders.ProcessRefund #1",   WorkItemState.Enqueued,  Now.AddMinutes(-24),  Now.AddMinutes(-23.9), QueueName: "critical"),
        ]);

        // 4 scheduled jobs
        jobs.AddRange([
            new($"job-{id++:D3}", "Reports.GenerateMonthly",   WorkItemState.Scheduled, Now.AddMinutes(-30),  null, Now.AddMinutes(60)),
            new($"job-{id++:D3}", "Maintenance.Vacuum",        WorkItemState.Scheduled, Now.AddMinutes(-35),  null, Now.AddMinutes(120)),
            new($"job-{id++:D3}", "Sync.FullBackup",           WorkItemState.Scheduled, Now.AddMinutes(-40),  null, Now.AddMinutes(180)),
            new($"job-{id++:D3}", "Agents.TrainModel",         WorkItemState.Scheduled, Now.AddMinutes(-45),  null, Now.AddMinutes(240)),
        ]);

        // 3 awaiting jobs (continuation chain)
        jobs.AddRange([
            new($"job-{id++:D3}", "Orders.FraudCheck",         WorkItemState.Awaiting,  Now.AddMinutes(-10),  QueueName: "critical",  ParentId: "job-001", ContinuationOptions: "OnSucceededJob"),
            new($"job-{id++:D3}", "Orders.SendConfirmation",   WorkItemState.Awaiting,  Now.AddMinutes(-12),  QueueName: "emails",    ParentId: "job-002", ContinuationOptions: "OnSucceededJob"),
            new($"job-{id++:D3}", "Orders.PrepareShipment",    WorkItemState.Awaiting,  Now.AddMinutes(-14),  QueueName: "default",   ParentId: "job-003", ContinuationOptions: "OnSucceededJob"),
        ]);

        // 2 deleted jobs
        jobs.AddRange([
            new($"job-{id++:D3}", "Orders.CancelledOrder",     WorkItemState.Deleted,   Now.AddMinutes(-20),  Now.AddMinutes(-19.9),null, null, null, null, Now.AddMinutes(-15), null, null, "default",   "User cancelled order"),
            new($"job-{id++:D3}", "Email.BouncedMessage",      WorkItemState.Deleted,   Now.AddMinutes(-25),  Now.AddMinutes(-24.9),null, null, null, null, Now.AddMinutes(-22), null, null, "emails",    "Mailbox does not exist"),
        ]);

        return jobs;
    }

    public static IReadOnlyList<WorkQueueItem> BuildQueues(IReadOnlyList<WorkItem> jobs)
    {
        var byQueue = jobs.Where(j => !string.IsNullOrWhiteSpace(j.QueueName))
                          .GroupBy(j => j.QueueName!)
                          .ToDictionary(g => g.Key, g => g.ToList());

        string[] queueNames = ["default", "critical", "emails", "imports", "agents"];
        var queues = new List<WorkQueueItem>();

        foreach (var name in queueNames)
        {
            var queueJobs = byQueue.GetValueOrDefault(name) ?? [];
            var enqueued = queueJobs.Count(j => j.State == WorkItemState.Enqueued);
            var fetched = queueJobs.Count(j => j.State == WorkItemState.Processing);
            var topJobs = queueJobs.Where(j => j.State is WorkItemState.Enqueued or WorkItemState.Processing)
                                   .Take(3)
                                   .ToList();
            queues.Add(new WorkQueueItem(name, enqueued, fetched, topJobs));
        }

        return queues;
    }

    public static IReadOnlyList<WorkServerItem> BuildServers()
    {
        return new List<WorkServerItem>
        {
            new("worker-01", 5, ["default", "critical"], Now.AddDays(-2), Now.AddSeconds(-15)),
            new("worker-02", 3, ["default", "emails", "agents"], Now.AddDays(-1), Now.AddSeconds(-45)),
            new("worker-03", 2, ["imports"], Now.AddHours(-12), Now.AddMinutes(-3))
        };
    }

    public static IReadOnlyList<WorkRecurringJobItem> BuildRecurringJobs()
    {
        return new List<WorkRecurringJobItem>
        {
            new("recur-daily-report",   "0 0 * * *",      "UTC",            "Reports.GenerateDaily",    Now.AddHours(12), Now.AddHours(-12), "job-004", WorkItemState.Succeeded, CreatedAt: Now.AddDays(-7)),
            new("recur-cleanup",        "0 */6 * * *",    "UTC",            "Maintenance.Cleanup",      Now.AddHours(6),  Now.AddHours(-6),  "job-010", WorkItemState.Succeeded, CreatedAt: Now.AddDays(-3)),
            new("recur-sync",           "*/15 * * * *",   "America/New_York", "Sync.ExternalData",      null,             null,              null,      null,                    Error: "Cron expression is invalid", CreatedAt: Now.AddDays(-1)),
            new("recur-notify",         "0 9 * * 1-5",    "Europe/London",  "Notifications.SendDigest", Now.AddHours(2),  Now.AddHours(-22), "job-015", WorkItemState.Succeeded, CreatedAt: Now.AddDays(-5))
        };
    }

    public static IReadOnlyList<WorkMetricItem> BuildMetrics(IReadOnlyList<WorkItem> jobs)
    {
        return new List<WorkMetricItem>
        {
            new("Succeeded",  jobs.Count(j => j.State == WorkItemState.Succeeded).ToString(),          Style: WorkMetricStyle.Success),
            new("Failed",     jobs.Count(j => j.State == WorkItemState.Failed).ToString(),             Style: WorkMetricStyle.Danger),
            new("Processing", jobs.Count(j => j.State == WorkItemState.Processing).ToString(),         Style: WorkMetricStyle.Info),
            new("Enqueued",   jobs.Count(j => j.State == WorkItemState.Enqueued).ToString(),           Style: WorkMetricStyle.Default),
            new("Scheduled",  jobs.Count(j => j.State == WorkItemState.Scheduled).ToString(),          Style: WorkMetricStyle.Warning),
            new("Awaiting",   jobs.Count(j => j.State == WorkItemState.Awaiting).ToString(),           Style: WorkMetricStyle.Default)
        };
    }

    public static WorkStatistics BuildStatistics(
        IReadOnlyList<WorkItem> jobs,
        IReadOnlyList<WorkQueueItem> queues,
        IReadOnlyList<WorkServerItem> servers,
        IReadOnlyList<WorkRecurringJobItem> recurring)
    {
        return new WorkStatistics(
            SucceededCount:  jobs.Count(j => j.State == WorkItemState.Succeeded),
            FailedCount:     jobs.Count(j => j.State == WorkItemState.Failed),
            ProcessingCount: jobs.Count(j => j.State == WorkItemState.Processing),
            EnqueuedCount:   jobs.Count(j => j.State == WorkItemState.Enqueued),
            ScheduledCount:  jobs.Count(j => j.State == WorkItemState.Scheduled),
            AwaitingCount:   jobs.Count(j => j.State == WorkItemState.Awaiting),
            DeletedCount:    jobs.Count(j => j.State == WorkItemState.Deleted),
            RecurringCount:  recurring.Count,
            RetriesCount:    jobs.Count(j => j.State == WorkItemState.Retry),
            ServersCount:    servers.Count,
            QueuesCount:     queues.Count,
            FetchedCount:    queues.Sum(q => q.Fetched));
    }

    public static WorkGraphData BuildGraphData()
    {
        return new WorkGraphData(
            DaySucceeded:  new[] { 45, 62, 38, 75, 55, 80, 42, 68, 50, 73, 60, 48 },
            DayFailed:     new[] { 2, 3, 1, 4, 2, 1, 3, 2, 1, 2, 3, 1 },
            DayDeleted:    new[] { 1, 0, 2, 1, 0, 1, 2, 0, 1, 1, 0, 2 },
            WeekSucceeded: new[] { 320, 450, 380, 520, 410, 490, 560 },
            WeekFailed:    new[] { 5, 8, 3, 10, 4, 6, 7 },
            WeekDeleted:   new[] { 3, 2, 5, 4, 1, 3, 2 });
    }

    public static IReadOnlyList<WorkStateHistory> BuildHistory(WorkItem job)
    {
        var history = new List<WorkStateHistory>
        {
            new("Created",  job.CreatedAt),
            new("Enqueued", job.EnqueuedAt ?? job.CreatedAt.AddSeconds(1)),
        };

        if (job.State >= WorkItemState.Processing)
            history.Add(new("Processing", job.StartedAt ?? job.CreatedAt.AddSeconds(5), DurationMs: job.Duration));

        if (job.State == WorkItemState.Succeeded)
            history.Add(new("Succeeded", job.SucceededAt ?? job.CreatedAt.AddSeconds(10), DurationMs: job.Duration));
        else if (job.State == WorkItemState.Failed)
            history.Add(new("Failed", job.FailedAt ?? job.CreatedAt.AddSeconds(10), job.Reason, DurationMs: job.Duration));
        else if (job.State == WorkItemState.Deleted)
            history.Add(new("Deleted", job.DeletedAt ?? job.CreatedAt.AddSeconds(10), job.Reason));

        return history;
    }
}
