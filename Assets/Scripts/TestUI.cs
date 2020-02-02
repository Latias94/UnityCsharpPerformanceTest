using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Tests;
using UnityEngine;
using UnityEngine.UI;

public enum TestType
{
    Exception,
    Strings,
    Arrays,
    Boxing,
    ForForeach,
    Struct,
    Memory,
    Instantiation,
    Properties
}

public class TestUI : MonoBehaviour
{
    public Text totalAText;
    public Text totalBText;
    public Text totalCText;
    public Slider totalASlider;
    public Slider totalBSlider;
    public Slider totalCSlider;
    public Text titleText;
    public Text descriptionText;
    public InputField inputField;
    public Dropdown dropdown;
    public Button testButton;
    public Toggle baseLineToggle;
    public Toggle resultToggle;

    private void Awake()
    {
        PopulateList();
        testButton.onClick.AddListener(OnClickTestButton);
        dropdown.onValueChanged.AddListener(DropdownIndexChanged);
        resultToggle.onValueChanged.AddListener(UpdatePerformanceResultUI);
        baseLineToggle.onValueChanged.AddListener(UpdateBaseLineUI);
        DropdownIndexChanged(0);
    }

    private void PopulateList()
    {
        string[] enumNames = Enum.GetNames(typeof(TestType));
        dropdown.options.Clear();
        dropdown.AddOptions(enumNames.ToList());
    }

    public void DropdownIndexChanged(int index)
    {
        baseLineToggle.isOn = true;
        TestType testType = (TestType) index;
        switch (testType)
        {
            case TestType.Exception:
                inputField.text = ExceptionTest.DefaultRepetitions.ToString();
                break;
            case TestType.Strings:
                inputField.text = StringsTest.DefaultRepetitions.ToString();
                break;
            case TestType.Arrays:
                inputField.text = ArraysTest.DefaultRepetitions.ToString();
                break;
            case TestType.Boxing:
                inputField.text = BoxingTest.DefaultRepetitions.ToString();
                break;
            case TestType.ForForeach:
                inputField.text = ForForeachTest.DefaultRepetitions.ToString();
                break;
            case TestType.Struct:
                inputField.text = StructTest.DefaultRepetitions.ToString();
                break;
            case TestType.Memory:
                inputField.text = MemoryTest.DefaultRepetitions.ToString();
                break;
            case TestType.Instantiation:
                inputField.text = InstantiationTest.DefaultRepetitions.ToString();
                break;
            case TestType.Properties:
                inputField.text = PropertiesTest.DefaultRepetitions.ToString();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnClickTestButton()
    {
        TestType testType = (TestType) dropdown.value;
        bool useBaseLine = baseLineToggle.isOn;
        RunTest(testType, useBaseLine);
    }

    private void RunTest(TestType testType, bool RunBaseLine = true)
    {
        PerformanceTest test = null;
        int iterations = int.Parse(inputField.text);
        switch (testType)
        {
            case TestType.Exception:
                test = new ExceptionTest {RunBaseline = RunBaseLine, Iterations = iterations};
                break;
            case TestType.Strings:
                test = new StringsTest {RunBaseline = RunBaseLine, Iterations = iterations};
                break;
            case TestType.Arrays:
                test = new ArraysTest {RunBaseline = RunBaseLine, Iterations = iterations};
                break;
            case TestType.Boxing:
                test = new BoxingTest {RunBaseline = RunBaseLine, Iterations = iterations};
                break;
            case TestType.ForForeach:
                test = new ForForeachTest {RunBaseline = RunBaseLine, Iterations = iterations};
                break;
            case TestType.Struct:
                test = new StructTest {RunBaseline = RunBaseLine, Iterations = iterations};
                break;
            case TestType.Memory:
                test = new MemoryTest {RunBaseline = RunBaseLine, Iterations = iterations};
                break;
            case TestType.Instantiation:
                test = new InstantiationTest {RunBaseline = RunBaseLine, Iterations = iterations};
                break;
            case TestType.Properties:
                test = new PropertiesTest {RunBaseline = RunBaseLine, Iterations = iterations};
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(testType), testType, null);
        }

        titleText.text = test.Name;
        descriptionText.text = test.Description;
        var averageResult = test.Measure();
        lastPerformanceResult = averageResult;
        UpdatePerformanceResultUI();

        var (perA, perB, perC) = GetPercentageOfResult(averageResult);
        totalASlider.value = perA;
        totalBSlider.value = perB;
        totalCSlider.value = perC;
    }

    private PerformanceResult lastPerformanceResult = null;

    private void UpdatePerformanceResultUI(bool uselessParameter = false)
    {
        if (lastPerformanceResult == null) return;
        bool showMs = resultToggle.isOn;
        if (showMs)
        {
            totalAText.text = $"TestA: {lastPerformanceResult.MillisecondsA} ms";
            totalBText.text = $"TestB: {lastPerformanceResult.MillisecondsB} ms";
            totalCText.text = $"TestC: {lastPerformanceResult.MillisecondsC} ms";
        }
        else
        {
            totalAText.text = $"TestA: {lastPerformanceResult.TickA} ticks";
            totalBText.text = $"TestB: {lastPerformanceResult.TickB} ticks";
            totalCText.text = $"TestC: {lastPerformanceResult.TickC} ticks";
        }

        totalAText.gameObject.SetActive(lastPerformanceResult.TickA != 0);
        totalBText.gameObject.SetActive(lastPerformanceResult.TickB != 0);
        totalCText.gameObject.SetActive(lastPerformanceResult.TickC != 0);
    }

    private void UpdateBaseLineUI(bool showBaseLine)
    {
        if (totalAText == null) return;
        totalAText.gameObject.SetActive(showBaseLine);
    }

    private (float, float, float) GetPercentageOfResult(PerformanceResult result)
    {
        float[] arr = {result.MillisecondsA, result.MillisecondsB, result.MillisecondsC};
        float max = arr.Max();
        return (result.MillisecondsA / max, result.MillisecondsB / max, result.MillisecondsC / max);
    }

    #region 反射 以后有空再写个自动生成UI类

    private const string TestFolderPath = "Assets\\Scripts\\Tests\\";

    private List<string> GetTestNameFromTestFolder()
    {
        List<string> result = new List<string>();
        if (!Directory.Exists(TestFolderPath))
        {
            Debug.LogError($"Test folder path: {TestFolderPath} not exit");
            return result;
        }

        DirectoryInfo directoryInfo = new DirectoryInfo(TestFolderPath);
        FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            string fileName = files[i].Name;
            if (fileName.EndsWith(".cs"))
            {
                fileName = fileName.Remove(fileName.Length - 3);
                Debug.Log($"Name: {fileName}");
                result.Add(fileName);
            }
        }

        return result;
    }

    private Type GetTestClassByReflection(string className)
    {
        Debug.Log("----");
        Assembly assembly =
            Assembly.LoadFrom($"{Environment.CurrentDirectory}\\Library\\ScriptAssemblies\\Assembly-CSharp.dll");
        dynamic obj = assembly.CreateInstance($"Tests.{className}");
        if (obj != null && obj is PerformanceTest)
        {
            return obj.GetType();
        }

        Debug.Log(obj == null
            ? $"Cannot find class: {className}"
            : $"class: {className} hasnt implement IPerformanceTest interface");
        return null;
    }

    #endregion
}