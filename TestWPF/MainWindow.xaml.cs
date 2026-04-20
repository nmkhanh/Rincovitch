using GraphShape.Controls;
using QuikGraph;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TestWPF
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      // ⚠️ QUAN TRỌNG: dùng object
      var graph = new BidirectionalGraph<object, IEdge<object>>();

      var leader = new Person("Leader", "Team Lead");
      var a = new Person("A", "Engineer");
      var b = new Person("B", "Engineer");

      graph.AddVertex(leader);
      graph.AddVertex(a);
      graph.AddVertex(b);

      graph.AddEdge(new Edge<object>(leader, a));
      graph.AddEdge(new Edge<object>(leader, b));

      graphLayout.Graph = graph;

      // 🔥 CUSTOM NODE ĐÚNG CÁCH
      graphLayout.Loaded += (s, e) =>
      {
        foreach (var vc in graphLayout.VertexList.Values)
        {
          if (vc.Vertex is Person p)
          {
            vc.Content = CreateNode(p);
          }
        }
      };
    }

    private UIElement CreateNode(Person p)
    {
      return new Border
      {
        Background = Brushes.LightBlue,
        BorderBrush = Brushes.DarkBlue,
        BorderThickness = new Thickness(1.5),
        CornerRadius = new CornerRadius(10),
        Padding = new Thickness(10),
        Child = new StackPanel
        {
          Children =
                    {
                        new TextBlock
                        {
                            Text = p.Name,
                            FontWeight = FontWeights.Bold,
                            HorizontalAlignment = HorizontalAlignment.Center
                        },
                        new TextBlock
                        {
                            Text = p.Role,
                            FontSize = 11,
                            HorizontalAlignment = HorizontalAlignment.Center
                        }
                    }
        }
      };
    }
  }

  public class Person
  {
    public string Name { get; }
    public string Role { get; }

    public Person(string name, string role)
    {
      Name = name;
      Role = role;
    }

    public override string ToString() => Name;
  }
}