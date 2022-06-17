using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MultiLayer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Bitmap bitmap;
        Random rnd = new Random();
        Graphics graphic;
        Pen pen;
        int size_picture = 32 * 32;
        // Вектор входных данных(x.i)
        List<int> input_list = new List<int>();

        // 3 скрытых слоя. На каждом слое по 3 нейрона.
        static int neurons_count_1 = 3;
        static int neurons_count_2 = 3;
        static int neurons_count_3 = 3;

        // Выходной слой. В выходном слое 2 нейрона.
        static int output_neorons_count = 2;

        // Для каждого нейрона на каждом слое свой массив весов.
        public double[,] first_weights = new double[neurons_count_1, 1024]; // Входное изображение 32x32
        public double[,] second_weights = new double[neurons_count_2, neurons_count_1];
        public double[,] third_weights = new double[neurons_count_3, neurons_count_2];
        public double[,] last_weights = new double[output_neorons_count, neurons_count_3];

        // Листы для хранения взвешенных весов.
        List<double> hidden_list_1 = new List<double>();
        List<double> hidden_list_2 = new List<double>();
        List<double> hidden_list_3 = new List<double>();
        List<double> output_list = new List<double>();

        // Графики обучения, тестирования, верификации отрисовываются по точкам. Точки рассчитываются и сохраняются в листы.
        List<double> learning_points = new List<double>();
        List<double> test_points = new List<double>();
        List<double> ver_points = new List<double>();

        // Идеальные значения для выхода сети. Массив применяется при использовании метода обучения с учителем.
        double[] target = new double[] { 0, 1 };

        // Рассчёт Delta для каждого нейрона.
        double[][] delta = { new double[neurons_count_1], new double[neurons_count_2], new double[neurons_count_3], new double[output_neorons_count]};
       
        // Скорость обучения.
        double learning_rate = 0.03;
        // Количество эпох обучения.
        int Epoch = 500;

        // Путь к каталогам.
        string[] allfiles_training, allfiles_testing, allfiles_verification;

        private void Form1_Load(object sender, EventArgs e) { }
        private void button1_Click(object sender, EventArgs e)
        {
            // Каталог для обучения
            string path = "";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                path = folderBrowserDialog1.SelectedPath;
                allfiles_training = Directory.GetFiles(path);
            }            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Обучение сети

            // Инициализация. Заполним массивы весов случайными значениями.
            for (int i = 0; i < neurons_count_1; i++)
                for (int j = 0; j < 1024; j++)
                    first_weights[i, j] = (int)rnd.Next(-1, 1 + 1);

            for (int i = 0; i < neurons_count_2; i++)
                for (int j = 0; j < neurons_count_1; j++)
                    second_weights[i, j] = (int)rnd.Next(-1, 1 + 1);

            for (int i = 0; i < neurons_count_3; i++)
                for (int j = 0; j < neurons_count_2; j++)
                    third_weights[i, j] = (int)rnd.Next(-1, 1 + 1);

            for (int i = 0; i < output_neorons_count; i++)
                for (int j = 0; j < neurons_count_3; j++)
                    last_weights[i, j] = (int)rnd.Next(-1, 1 + 1);

            // Вычисление сигмоид для каждого нейрона. Обучение проходит в несколько эпох.
            // Проход осуществляется по всем обучающим картинкам.
            for (int file_count = 0; file_count < allfiles_training.Length; file_count++)
            {
                for (int count = 0; count < Epoch; count++)
                {
                    // Получение входных данных.
                    input_list.Clear();
                    string filename = allfiles_training[file_count];
                    bitmap = new Bitmap(filename);
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        for (int y = 0; y < bitmap.Height; y++)
                        {
                            int n = (bitmap.GetPixel(x, y).R);                            
                            if (bitmap.GetPixel(x, y).ToArgb() == Color.Black.ToArgb()) n = 1;
                            else n = 0;
                            input_list.Add(n);
                        }
                    }
                    hidden_list_1.Clear();
                    hidden_list_2.Clear();
                    hidden_list_3.Clear();
                    output_list.Clear();

                    // Вычисление точек сигмоиды для нейронов первого скрытого слоя.
                    for (int i = 0; i < neurons_count_1; i++)
                    {
                        double res = 0;
                        for (int j = 0; j < bitmap.Width * bitmap.Height; j++)
                        {
                            res += input_list[j] * first_weights[i, j];
                        }
                        double output = 1 / (1 + Math.Exp(-res));
                        hidden_list_1.Add(output);
                    }

                    // Вычисление точек сигмоиды для нейронов второго скрытого слоя.
                    for (int i = 0; i < neurons_count_2; i++)
                    {
                        double res = 0;
                        for (int j = 0; j < neurons_count_1; j++)
                        {
                            res += hidden_list_1[j] * second_weights[i, j];
                        }
                        double output = 1 / (1 + Math.Exp(-res));
                        hidden_list_2.Add(output);
                    }

                    // Вычисление точек сигмоиды для нейронов третьего скрытого слоя.
                    for (int i = 0; i < neurons_count_3; i++)
                    {
                        double res = 0;
                        for (int j = 0; j < neurons_count_2; j++)
                        {
                            res += hidden_list_2[j] * third_weights[i, j];
                        }
                        double output = 1 / (1 + Math.Exp(-res));
                        hidden_list_3.Add(output);
                    }

                    // Вычисление точек сигмоиды для нейронов выходгого слоя.
                    for (int i = 0; i < output_neorons_count; i++)
                    {
                        double res = 0.0;
                        for (int j = 0; j < neurons_count_3; j++)
                        {
                            res += hidden_list_3[j] * last_weights[i, j];
                        }
                        double output = 1 / (1 + Math.Exp(-res));
                        output_list.Add(output);
                    }

                    // Вычисление дельты для нейронов выходного слоя.
                    for (int k = 0; k < output_neorons_count; k++)
                        delta[3][k] = (target[k] - output_list[k]) * output_list[k] * (1 - output_list[k]);

                    // Вычисление дельты для нейронов третьего скрытого слоя.
                    for (int k = 0; k < neurons_count_3; k++)
                        delta[2][k] = hidden_list_3[k] * (1 - hidden_list_3[k]) * (delta[3][0] * last_weights[0, k] + delta[3][1] * last_weights[1, k]);

                    // Вычисление дельты для нейронов второго скрытого слоя.
                    for (int k = 0; k < neurons_count_2; k++)
                        delta[1][k] = hidden_list_2[k] * (1 - hidden_list_2[k]) * (delta[2][0] * third_weights[0, k] + delta[2][1] * third_weights[1, k] + delta[2][2] * third_weights[2, k]);
                    
                    // Вычисление дельты для нейронов первого скрытого слоя.
                    for (int k = 0; k < neurons_count_1; k++)
                        delta[0][k] = hidden_list_1[k] * (1 - hidden_list_1[k]) * (delta[1][0] * second_weights[0, k] + delta[1][1] * second_weights[1, k] + delta[1][2] * second_weights[2, k]);

                    // Изменение весов нейронов.

                    // Изменение весов первого скрытого слоя.
                    for (int i = 0; i < neurons_count_1; i++)
                    {
                        double delta_weight = 0;
                        for (int j = 0; j < bitmap.Width * bitmap.Height; j++)
                        {
                            delta_weight = learning_rate * input_list[j] * delta[0][i];
                            first_weights[i, j] += delta_weight;
                        }
                    }
                    // Изменение весов второго скрытого слоя.
                    for (int i = 0; i < neurons_count_2; i++)
                    {
                        double delta_weight = 0;
                        for (int j = 0; j < neurons_count_1; j++)
                        {
                            delta_weight = learning_rate * hidden_list_1[j] * delta[1][i];
                            second_weights[i, j] += delta_weight;
                        }
                    }
                    // Изменение весов третьего скрытого слоя.
                    for (int i = 0; i < neurons_count_3; i++)
                    {
                        double delta_weight = 0;
                        for (int j = 0; j < neurons_count_2; j++)
                        {
                            delta_weight = learning_rate * hidden_list_2[j] * delta[2][i];
                            third_weights[i, j] += delta_weight;
                        }
                    }
                    // Изменение весов выходного слоя.
                    for (int i = 0; i < output_neorons_count; i++)
                    {
                        double delta_weight = 0;
                        for (int j = 0; j < neurons_count_1; j++)
                        {
                            delta_weight = learning_rate * hidden_list_3[j] * delta[3][i];
                            last_weights[i, j] += delta_weight;
                        }
                    }

                    hidden_list_1.Clear();
                    hidden_list_2.Clear();
                    hidden_list_3.Clear();
                    output_list.Clear();

                    // Отрисовка ошибки при распозновании

                    // Вычисление сигмоид для первого скрытого слоя
                    for (int i = 0; i < neurons_count_1; i++)
                    {
                        double res = 0;
                        for (int j = 0; j < bitmap.Width * bitmap.Height; j++)
                        {
                            res += input_list[j] * first_weights[i, j];
                        }
                        double output = 1 / (1 + Math.Exp(-res));
                        hidden_list_1.Add(output);
                    }
                    // Вычисление сигмоид для второго скрытого слоя
                    for (int i = 0; i < neurons_count_2; i++)
                    {
                        double res = 0;
                        for (int j = 0; j < neurons_count_1; j++)
                        {
                            res += hidden_list_1[j] * second_weights[i, j];
                        }
                        double output = 1 / (1 + Math.Exp(-res));
                        hidden_list_2.Add(output);
                    }
                    // Вычисление сигмоид для третьего скрытого слоя
                    for (int i = 0; i < neurons_count_3; i++)
                    {
                        double res = 0;
                        for (int j = 0; j < neurons_count_2; j++)
                        {
                            res += hidden_list_2[j] * third_weights[i, j];
                        }
                        double output = 1 / (1 + Math.Exp(-res));
                        hidden_list_3.Add(output);
                    }
                    // Вычисление сигмоид для последнего слоя
                    for (int i = 0; i < output_neorons_count; i++)
                    {
                        double res = 0.0;
                        for (int j = 0; j < neurons_count_3; j++)
                        {
                            res += hidden_list_3[j] * last_weights[i, j];
                        }
                        double output = 1 / (1 + Math.Exp(-res));
                        output_list.Add(output);
                    }
                }
                learning_points.Add(output_list[1]);
            }

            // Отрисовка графика ошибки распознования
            graphic = pictureBox2.CreateGraphics();
            graphic.Clear(Color.White);
            PointF[] points = new PointF[learning_points.Count];
            pen = new Pen(Color.Red, 3f);

            for (int count = 0; count < learning_points.Count; count++)
            {
                points[count] = new PointF(count, (float)(target[1] - learning_points[count]));
            }
            graphic.DrawLines(pen, points);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Каталог для тестирования
            string path = "";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                path = folderBrowserDialog1.SelectedPath;
                allfiles_testing = Directory.GetFiles(path);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Каталог для валидации
            string path = "";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                path = folderBrowserDialog1.SelectedPath;
                allfiles_verification = Directory.GetFiles(path);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Верифицировать данные.
            // После считывание картинки происходит вычисление сигмоид для нейронов.
            // Результат распознования по каждой картинке записывается в ver_points[]. Затем отрисовывается график ошибки верификации.

            for (int file_count = 0; file_count < allfiles_verification.Length; file_count++)
            {
                // Получение входных данных.

                input_list.Clear();
                string filename = allfiles_training[file_count];
                bitmap = new Bitmap(filename);

                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        int n = (bitmap.GetPixel(x, y).R);
                        if (bitmap.GetPixel(x, y).ToArgb() == Color.Black.ToArgb()) n = 1;
                        else n = 0;
                        input_list.Add(n);
                    }
                }

                hidden_list_1.Clear();
                hidden_list_2.Clear();
                hidden_list_3.Clear();
                output_list.Clear();

                // Вычисление точек сигмоиды для нейронов первого скрытого слоя.
                for (int i = 0; i < neurons_count_1; i++)
                {
                    double res = 0;
                    for (int j = 0; j < bitmap.Width * bitmap.Height; j++)
                    {
                        res += input_list[j] * first_weights[i, j];
                    }
                    double output = 1 / (1 + Math.Exp(-res));
                    hidden_list_1.Add(output);
                }

                // Вычисление точек сигмоиды для нейронов второго скрытого слоя.
                for (int i = 0; i < neurons_count_2; i++)
                {
                    double res = 0;
                    for (int j = 0; j < neurons_count_1; j++)
                    {
                        res += hidden_list_1[j] * second_weights[i, j];
                    }
                    double output = 1 / (1 + Math.Exp(-res));
                    hidden_list_2.Add(output);
                }

                // Вычисление точек сигмоиды для нейронов третьего скрытого слоя.
                for (int i = 0; i < neurons_count_3; i++)
                {
                    double res = 0;
                    for (int j = 0; j < neurons_count_2; j++)
                    {
                        res += hidden_list_2[j] * third_weights[i, j];
                    }
                    double output = 1 / (1 + Math.Exp(-res));
                    hidden_list_3.Add(output);
                }

                // Вычисление точек сигмоиды для нейронов выходгого слоя.
                for (int i = 0; i < output_neorons_count; i++)
                {
                    double res = 0.0;
                    for (int j = 0; j < neurons_count_3; j++)
                    {
                        res += hidden_list_3[j] * last_weights[i, j];
                    }
                    double output = 1 / (1 + Math.Exp(-res));
                    output_list.Add(output);
                }
            }
            ver_points.Add(output_list[1]);

            // Отрисовка графика ошибки верификации
            graphic = pictureBox2.CreateGraphics();
            graphic.Clear(Color.White);
            PointF[] points = new PointF[ver_points.Count];
            pen = new Pen(Color.Red, 3f);

            for (int count = 0; count < ver_points.Count; count++)
            {
                points[count] = new PointF(count, (float)(target[1] - ver_points[count]));
            }
            graphic.DrawLines(pen, points);
        }
    }
}
