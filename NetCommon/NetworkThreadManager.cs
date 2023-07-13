namespace Networking; 

public static class NetworkThreadManager {
    private static readonly List<Action> executions     = new();
    private static readonly List<Action> execute_copies = new();

    public static void executeMain(Action action) {
        lock (executions)
            executions.Add(action);
    }
    
    public static void executeActions() {
        if (executions.Count <= 0)
            return;

        execute_copies.Clear();
        
        lock (executions) {
            execute_copies.AddRange(executions);
            executions.Clear();
        }

        foreach (var execute in execute_copies)
            execute();
    }
}