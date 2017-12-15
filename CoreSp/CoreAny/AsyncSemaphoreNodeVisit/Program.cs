using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncSemaphoreNodeVisit
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Node tree = InitTree();
            TreeVisitor tv = new TreeVisitor();
            tv.Visit(tree);

            Task a =  AsyncTreeVisitor.VisitAsync(tree);
            a.Wait();

            Console.Read();
        }

        public static Node InitTree()
        {
            var A = new Node { Value = "A" };
            var B = new Node { Value = "B" };
            var C = new Node { Value = "C" };
            var D = new Node { Value = "D" };
            var E = new Node { Value = "E" };
            var F = new Node { Value = "F" };
            var G = new Node { Value = "G" };
            A.Nodes.AddLast(B);
            A.Nodes.AddLast(C);
            A.Nodes.AddLast(D);
            B.Nodes.AddLast(A);
            B.Nodes.AddLast(D);
            C.Nodes.AddLast(A);
            D.Nodes.AddLast(A);
            D.Nodes.AddLast(B);
            D.Nodes.AddLast(E);
            E.Nodes.AddLast(D);
            E.Nodes.AddLast(F);
            F.Nodes.AddLast(E);
            F.Nodes.AddLast(G);
            G.Nodes.AddLast(F);

            return A;
        }
    }

    public class Node
    {
        public string Value { get; set; }
        public LinkedList<Node> Nodes { get; set; } = new LinkedList<Node>();
    }

    public class TreeVisitor
    {
        private static HashSet<int> visited = new HashSet<int>();

        public void Visit(Node n)
        {
            lock (visited)
            {
                if (visited.Contains(n.GetHashCode()))
                {
                    return;
                }
                visited.Add(n.GetHashCode());
            }
            Console.WriteLine($"{n.Value} ");
            Parallel.ForEach(n.Nodes, x =>
            {
                Visit(x);
            });
        }
    }

    public class AsyncSemaphore
    {
        private int _totalCount = 0;
        private int _finishedCount = 0;

        private readonly List<TaskCompletionSource<bool>> _waiters = new List<TaskCompletionSource<bool>>();
        private readonly static Task _completed = Task.FromResult(true);

        public void AddTaskCount(int count)
        {
            _totalCount += count;
        }

        public Task WaitAsync()
        {
            lock (_waiters)
            {
                if (_finishedCount == _totalCount)
                {
                    return _completed;
                }
                var waiter = new TaskCompletionSource<bool>();
                _waiters.Add(waiter);
                return waiter.Task;
            }
        }

        public void Release()
        {
            lock (_waiters)
            {
                _finishedCount++;
                if (_finishedCount == _totalCount)
                {
                    Parallel.ForEach(_waiters, x =>
                    {
                        x.SetResult(true);
                    });
                    _waiters.Clear();
                }
            }
        }
    }

    public class AsyncTreeVisitor
    {
        private static HashSet<int> visited = new HashSet<int>();
        private static Dictionary<int, AsyncSemaphore> lockers = new Dictionary<int, AsyncSemaphore>();

        public AsyncTreeVisitor()
        {
            lockers.Add(0, new AsyncSemaphore());
            lockers[0].AddTaskCount(1);
        }

        public static async Task VisitAsync(Node n, int deep = 0)
        {
            lock (visited)
            {
                if (visited.Contains(n.GetHashCode()))
                {
                    lockers[deep].Release();
                    return;
                }
                visited.Add(n.GetHashCode());
            }
            lock (lockers)
            {
                if (!lockers.ContainsKey(deep + 1))
                {
                    lockers.Add(deep + 1, new AsyncSemaphore());
                }
            }
            Console.Write($"{n.Value} ");
            lockers[deep + 1].AddTaskCount(n.Nodes.Count);
            lockers[deep].Release();
            await lockers[deep].WaitAsync();
            Parallel.ForEach(n.Nodes, async x =>
            {
                await VisitAsync(x, deep + 1);
            });
        }
    }
}
