using System;
using System.Diagnostics;
using HugMun.Core;
using HugMun.Data;
using HugMun.Preprocessing;
using HugMun.Reasoning;
using HugMun.Validation;

namespace HugMun.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var cases = FileProvider.Load<BreastCancerData>("");
            var folds = CrossValidator.CreateFolds(cases, 5, true);

            var watch = Stopwatch.StartNew();
            for (var k = 1; k < 31; k++)
            {
                var k1 = k;
                var result = CrossValidator.ValidateInParallel<int>(folds, cases, ReasonerBuilder, PipelineBuilder);
                Console.WriteLine($"K: {k1}, AverageTime: {result.ValidationTime}, Accuracy: {result.Accuracy():0.####}");

                TransformerPipeline PipelineBuilder() =>
                    new TransformerPipeline()
                        .Add(new MinMaxNormalizer());

                Reasoner ReasonerBuilder()
                {
                    var cycle = new ReasoningCycle()
                        .AddRetriever(new LshRetriever(15, 3))
                        .AddRetriever(new SimilarityRetriever(new MinkowskiDistance(2), 35))
                        .SetReuser(new KnnReuser(k1));
                    return new Reasoner(cycle);
                }
            }

            watch.Stop();
            Console.WriteLine($"TotalExecutionTime: {watch.ElapsedMilliseconds}, AverageFoldTime: {watch.ElapsedMilliseconds / 30d}");
            Console.ReadLine();
        }
    }

    public class DiabetesData
    {
        [CaseId]
        public string Id { get; set; }
        [CaseAttribute]
        public double Pregnancies { get; set; }
        [CaseAttribute]
        public double Glucose { get; set; }
        [CaseAttribute]
        public double BloodPressure { get; set; }
        [CaseAttribute]
        public double SkinThickness { get; set; }
        [CaseAttribute]
        public double Insulin { get; set; }
        [CaseAttribute]
        public double BMI { get; set; }
        [CaseAttribute]
        public double DiabetesPedigreeFunction { get; set; }
        [CaseAttribute]
        public double Age { get; set; }
        [CaseSolution]
        public int Outcome { get; set; }
    }

    public class CardioData
    {
        [CaseId]
        public string id { get; set; }
        [CaseAttribute]
        public double age { get; set; }
        [CaseAttribute]
        public double gender_m { get; set; }
        [CaseAttribute]
        public double gender_f { get; set; }
        [CaseAttribute]
        public double height { get; set; }
        [CaseAttribute]
        public double weight { get; set; }
        [CaseAttribute]
        public double ap_hi { get; set; }
        [CaseAttribute]
        public double ap_lo { get; set; }
        [CaseAttribute]
        public double cholesterol { get; set; }
        [CaseAttribute]
        public double gluc { get; set; }
        [CaseAttribute]
        public double smoke { get; set; }
        [CaseAttribute]
        public double alco { get; set; }
        [CaseAttribute]
        public double active { get; set; }
        [CaseSolution]
        public int cardio { get; set; }
    }

    public class LiverData
    {
        [CaseId]
        public string Id { get; set; }
        [CaseAttribute]
        public double Age { get; set; }
        [CaseAttribute]
        public double Gender_M { get; set; }
        [CaseAttribute]
        public double Gender_F { get; set; }
        [CaseAttribute]
        public double Total_Bilirubin { get; set; }
        [CaseAttribute]
        public double Direct_Bilirubin { get; set; }
        [CaseAttribute]
        public double Alkaline_Phosphotase { get; set; }
        [CaseAttribute]
        public double Alamine_Aminotransferase { get; set; }
        [CaseAttribute]
        public double Aspartate_Aminotransferase { get; set; }
        [CaseAttribute]
        public double Total_Protiens { get; set; }
        [CaseAttribute]
        public double Albumin { get; set; }
        [CaseAttribute]
        public double Albumin_and_Globulin_Ratio { get; set; }
        [CaseSolution]
        public int Dataset { get; set; }
    }

    public class MammographyData
    {
        [CaseId]
        public string Id { get; set; }
        [CaseAttribute]
        public double BI_RADS { get; set; }
        [CaseAttribute]
        public double Age { get; set; }
        [CaseAttribute]
        public double Density { get; set; }
        [CaseAttribute]
        public double Shape1 { get; set; }
        [CaseAttribute]
        public double Shape2 { get; set; }
        [CaseAttribute]
        public double Shape3 { get; set; }
        [CaseAttribute]
        public double Shape4 { get; set; }
        [CaseAttribute]
        public double Margin1 { get; set; }
        [CaseAttribute]
        public double Margin2 { get; set; }
        [CaseAttribute]
        public double Margin3 { get; set; }
        [CaseAttribute]
        public double Margin4 { get; set; }
        [CaseAttribute]
        public double Margin5 { get; set; }
        [CaseSolution]
        public int Severity { get; set; }
    }

    public class BreastCancerData
    {
        [CaseId]
        public string id { get; set; }
        [CaseAttribute]
        public double radius_mean { get; set; }
        [CaseAttribute]
        public double texture_mean { get; set; }
        [CaseAttribute]
        public double perimeter_mean { get; set; }
        [CaseAttribute]
        public double area_mean { get; set; }
        [CaseAttribute]
        public double smoothness_mean { get; set; }
        [CaseAttribute]
        public double compactness_mean { get; set; }
        [CaseAttribute]
        public double concavity_mean { get; set; }
        [CaseAttribute]
        public double concavepoints_mean { get; set; }
        [CaseAttribute]
        public double symmetry_mean { get; set; }
        [CaseAttribute]
        public double fractal_dimension_mean { get; set; }
        [CaseAttribute]
        public double radius_se { get; set; }
        [CaseAttribute]
        public double texture_se { get; set; }
        [CaseAttribute]
        public double perimeter_se { get; set; }
        [CaseAttribute]
        public double area_se { get; set; }
        [CaseAttribute]
        public double smoothness_se { get; set; }
        [CaseAttribute]
        public double compactness_se { get; set; }
        [CaseAttribute]
        public double concavity_se { get; set; }
        [CaseAttribute]
        public double concavepoints_se { get; set; }
        [CaseAttribute]
        public double symmetry_se { get; set; }
        [CaseAttribute]
        public double fractal_dimension_se { get; set; }
        [CaseAttribute]
        public double radius_worst { get; set; }
        [CaseAttribute]
        public double texture_worst { get; set; }
        [CaseAttribute]
        public double perimeter_worst { get; set; }
        [CaseAttribute]
        public double area_worst { get; set; }
        [CaseAttribute]
        public double smoothness_worst { get; set; }
        [CaseAttribute]
        public double compactness_worst { get; set; }
        [CaseAttribute]
        public double concavity_worst { get; set; }
        [CaseAttribute]
        public double concavepoints_worst { get; set; }
        [CaseAttribute]
        public double symmetry_worst { get; set; }
        [CaseAttribute]
        public double fractal_dimension_worst { get; set; }
        [CaseSolution]
        public int diagnosis { get; set; }
    }
}

