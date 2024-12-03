using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace TDGResponseSheetTest.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableCollection<TestVM> _tests;
    [ObservableProperty] private FlatTreeDataGridSource<TestVM[]> _testSource;

    private WrapperSelector? _wrapSelector;
    private TestVMSelector? _testSelector;

    public MainViewModel()
    {
        // Columns
        Tests = new(
            [
                new TestVM() { Value = "Question", TypeOfTest = TestType.Text },
                new TestVM() { Value = "Order Answered", TypeOfTest = TestType.Text },
                new TestVM() { Value = "Completed", TypeOfTest = TestType.Text },
            ]);

        // Data
        TestSource = new(
            [
                [new TestVM() { Value = "Question #1 Answer", TypeOfTest = TestType.Text}, new TestVM() { Value = 0, TypeOfTest = TestType.Number}, new TestVM() { Value = false, TypeOfTest = TestType.Bool}],
                [new TestVM() { Value = null, TypeOfTest = TestType.Number}, new TestVM() { Value = 0, TypeOfTest = TestType.Number}, new TestVM() { Value = false, TypeOfTest = TestType.Bool}],
                [new TestVM() { Value = true, TypeOfTest = TestType.Bool}, new TestVM() { Value = 0, TypeOfTest = TestType.Number}, new TestVM() { Value = false, TypeOfTest = TestType.Bool}],
            ]);
    }

    [RelayCommand]
    public void LoadData()
    {
        for (var i = 0; i < _tests.Count; i++)
        {
            var index = i;
            TestSource.Columns.Add(CreateColumn(_tests[i].Value.ToString(), index));
        }
    }

    private IColumn<TestVM[]> CreateColumn(string header, int index)
    {
        _testSelector = new()
        {
            TextTemplate = new FuncDataTemplate<TestVM>((data, _) =>
            {
                return new TextBox
                {
                    [!TextBox.TextProperty] = new Binding($"[{index}].Value"),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                };
            }),
            NumberTemplate = new FuncDataTemplate<TestVM>((data, _) =>
            {
                return new NumericUpDown()
                {
                    [!NumericUpDown.ValueProperty] = new Binding($"[{index}].Value"),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                };
            }),
            BoolTemplate = new FuncDataTemplate<TestVM>((data, _) =>
            {
                return new CheckBox()
                {
                    [!CheckBox.IsCheckedProperty] = new Binding($"[{index}].Value"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
            })
        };

        _wrapSelector = new()
        {
            WrapTemplate = new FuncDataTemplate<TestVM[]>((data, _) =>
            {
                return _testSelector.Build(data[index]);
            })
        };

        return new TemplateColumn<TestVM[]>(header, _wrapSelector, width: GridLength.Star);
    }
}

public class TestVM : ObservableObject
{
    public object? Value { get; set; }
    public TestType TypeOfTest { get; set; }
}

public enum TestType
{
    Text,
    Number,
    Bool,
}

public class WrapperSelector : IDataTemplate
{
    public IDataTemplate WrapTemplate { get; set; }

    public Control? Build(object? param)
    {
        return WrapTemplate.Build(param);
    }

    public bool Match(object? param)
    {
        return param is TestVM[];
    }
}

public class TestVMSelector : IDataTemplate
{

    public IDataTemplate TextTemplate { get; set; }
    public IDataTemplate NumberTemplate { get; set; }
    public IDataTemplate BoolTemplate { get; set; }

    public Control? Build(object? param)
    {
        if (param is not TestVM tv)
            return null;

        return tv.TypeOfTest switch
        {
            TestType.Text => TextTemplate.Build(param),
            TestType.Number => NumberTemplate.Build(param),
            TestType.Bool => BoolTemplate.Build(param),
            _ => null
        };
    }

    public bool Match(object? data)
    {
        return data is TestVM;
    }
}