// See https://aka.ms/new-console-template for more information

using System.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

Dictionary<int, List<double>> nodes = new Dictionary<int, List<double>>();

List<(int, int, double)> resitors = new List<(int, int, double)>();

/*
 *File format
 *# of nodes
 *# of resistors
 *$N1 $N2 $R1
 * 
 * 
 * $N1 and $N2 are the 1 based node number and $N2 should always be greater than $N1
 * First node and last node should be the the first and last terminal of the ciruit
 */

using (var stream = File.OpenText("nodes.txt"))
{

    var line = stream.ReadLine(); 
    int num_nodes = int.Parse(line);
    var a = DenseMatrix.Create(num_nodes, num_nodes, 0);
    var b = DenseMatrix.Create(num_nodes, 1, 0);
    b[0, 0] = 1;
    a[0, 0] = 1;
    a[num_nodes - 1, num_nodes -1] = 1;
    line = stream.ReadLine();
    int num_resistors = int.Parse(line);

    for (int i = 0; i < num_resistors; i++)
    {
        line = stream.ReadLine();
        var values = line.Split();
        int node1 = int.Parse(values[0])-1;
        int node2 = int.Parse(values[1])-1;
        double resistor = double.Parse(values[2]);
        resitors.Add((node1, node2, resistor));
        
        double inverse_resisotr = 1 / resistor;
        if(node1 != 0 && node1 != num_nodes-1 )
            a[node1, node1] -= inverse_resisotr;
        if (node2 != 0 && node2 != num_nodes - 1)
           a[node2, node2] -= inverse_resisotr;
        if(node1 != 0 && node1 != num_nodes - 1)
           a[node1, node2] += inverse_resisotr;
        if(node2 != 0 && node2 != num_nodes-1)
            a[node2, node1] += inverse_resisotr;

    }
    stream.Close();
    Console.WriteLine(a);
    var test1= a.Determinant();
  
    var x = a.Solve(b);
    Console.WriteLine(x);

    Console.WriteLine("Current through resitors");
    double current_sum1 = 0;
    double current_sum2 = 0;

    foreach(var rv in resitors)
    {
        (var node1, var node2, double r) = rv;
        double vdelta = x[node1, 0] - x[node2,0];
        double current = vdelta / r;

        if (node1 == 0) current_sum1 += current;
        if (node2 == num_nodes - 1) current_sum2 += current;

        Console.WriteLine($"{node1 + 1} {node2 + 1} {r} {vdelta} {current}");
        
    }
    Console.WriteLine($"{current_sum1} {current_sum2} R={1/current_sum2}");


}
