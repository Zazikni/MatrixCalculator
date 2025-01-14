using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixCalculator
{
    public class Matrix
    {
        private int[,] data;

        public Matrix(int rows, int cols)
        {
            data = new int[rows, cols];
        }
        public Matrix(int[,] matrix)
        {
            data = matrix;
        }
        public Matrix(int rows, int cols, bool fill)
        {
            data = new int[rows, cols];
            Random random = new Random();
            if (fill)
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        this[i, j] = random.Next(10); // Заполнение случайными значениями от 0 до 9
                    }
                }
            }

        }

        public int Rows => data.GetLength(0);
        public int Cols => data.GetLength(1);

        public int this[int row, int col]
        {
            get { return data[row, col]; }
            set { data[row, col] = value; }
        }

        public void Print()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    Console.Write(data[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
        #region MultiplyMatrixSequential
        public static Matrix MultiplyMatrixSequential(Matrix a, Matrix b)
        {
            if (a.Cols != b.Rows)
            {
                throw new ArgumentException("Number of columns in the first matrix must be equal to the number of rows in the second matrix.");
            }

            Matrix result = new Matrix(a.Rows, b.Cols);

            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < b.Cols; j++)
                {
                    result[i, j] = 0;
                    for (int k = 0; k < a.Cols; k++)
                    {
                        result[i, j] += a[i, k] * b[k, j];
                    }
                }
            }


            return result;
        }
        #endregion MultiplyMatrixSequential

        #region MultiplyParallelUsingParralelFor
        public static Matrix MultiplyParallelUsingParralelFor(Matrix a, Matrix b)
        {
            if (a.Cols != b.Rows)
            {
                throw new ArgumentException("Number of columns in the first matrix must be equal to the number of rows in the second matrix.");
            }

            Matrix result = new Matrix(a.Rows, b.Cols);

            Parallel.For(0, a.Rows, i =>
            {
                for (int j = 0; j < b.Cols; j++)
                {
                    result[i, j] = 0;
                    for (int k = 0; k < a.Cols; k++)
                    {
                        result[i, j] += a[i, k] * b[k, j];
                    }
                }
            });

            return result;
        }
        #endregion MultiplyParallelUsingParralelFor

        #region MultiplyMatrixUsingRuleOneThreadOneRow
        private static void ThreadMethod(object state)
        {

            var parameters_list = (List<ThreadParameters>)state;
            foreach (ThreadParameters parameters in parameters_list)
            {
                int i_index = parameters.IIndex;
                Matrix a_ref = parameters.ARef;
                Matrix b_ref = parameters.BRef;
                Matrix result_ref = parameters.ResultRef;

                for (int j = 0; j < b_ref.Cols; j++)
                {
                    result_ref[i_index, j] = 0;
                    for (int k = 0; k < a_ref.Cols; k++)
                    {
                        result_ref[i_index, j] += a_ref[i_index, k] * b_ref[k, j];
                    }
                }
            }


        }
        private class ThreadParameters
        {
            public int IIndex { get; set; }
            public Matrix ARef { get; set; }
            public Matrix BRef { get; set; }
            public Matrix ResultRef { get; set; }
        }
        public static Matrix MultiplyMatrixUsingRuleOneThreadOneRow(Matrix a, Matrix b, int threads_count)
        {
            if (a.Cols != b.Rows)
            {
                throw new ArgumentException("Number of columns in the first matrix must be equal to the number of rows in the second matrix.");
            }

            Matrix result = new Matrix(a.Rows, b.Cols);
            Thread[] threads_list = new Thread[threads_count];
            List<ThreadParameters>[] threads_data = new List<ThreadParameters>[threads_count];

            for (int i = 0; i < threads_count; i++)
            {
                threads_data[i] = new List<ThreadParameters>();
            }

            for (int i = 0; i < a.Rows; i++)
            {
                int index = i % threads_count;
                threads_data[index].Add(new ThreadParameters { IIndex = i, ARef = a, BRef = b, ResultRef = result });
            }

            for (int i = 0; i < threads_count; i++)
            {
                threads_list[i] = new Thread(ThreadMethod);
                threads_list[i].Start(threads_data[i]);
            }

            foreach (Thread thread in threads_list)
            {
                thread.Join();
            }

            return result;
        }



        #endregion MultiplyMatrixUsingRuleOneThreadOneRow

        #region MultiplyMatrixUsingTreadsAndDividingMatrixIntoRanges







        private class ThreadParametersUsingTreadsAndDividingMatrixIntoRanges
        {
            public int RowStartIndex { get; set; }
            public int RowEndIndex { get; set; }
            public int ColumnStartIndex { get; set; }
            public int ColumnEndIndex { get; set; }
            public Matrix ARef { get; set; }
            public Matrix BRef { get; set; }
            public Matrix ResultRef { get; set; }
        }

        private static void ThreadMethodUsingTreadsAndDividingMatrixIntoRanges(object state)
        {
            var parameters = (ThreadParametersUsingTreadsAndDividingMatrixIntoRanges)state;
            int rowStartIndex = parameters.RowStartIndex;
            int rowEndIndex = parameters.RowEndIndex;
            int columnStartIndex = parameters.ColumnStartIndex;
            int columnEndIndex = parameters.ColumnEndIndex;
            Matrix aRef = parameters.ARef;
            Matrix bRef = parameters.BRef;
            Matrix resultRef = parameters.ResultRef;

            for (int i = rowStartIndex; i <= rowEndIndex; i++)
            {
                for (int j = columnStartIndex; j <= columnEndIndex; j++)
                {
                    int sum = 0;
                    for (int k = 0; k < aRef.Cols; k++)
                    {
                        sum += aRef[i, k] * bRef[k, j];
                    }
                    resultRef[i, j] = sum;
                }
            }
        }

        static (int, int, int, int)[] GetMatrixBlockRanges(Matrix result, int n)
        {
            int rows = result.Rows;
            int cols = result.Cols;
            int blocksPerRow = (int)Math.Sqrt(n);
            int blocksPerCol = n / blocksPerRow;

            var ranges = new (int, int, int, int)[n];

            int blockSizeRows = rows / blocksPerRow;
            int blockSizeCols = cols / blocksPerCol;

            int index = 0;
            for (int i = 0; i < blocksPerRow; i++)
            {
                for (int j = 0; j < blocksPerCol; j++)
                {
                    int rowStart = i * blockSizeRows;
                    int rowEnd = (i == blocksPerRow - 1) ? rows - 1 : (i + 1) * blockSizeRows - 1;
                    int colStart = j * blockSizeCols;
                    int colEnd = (j == blocksPerCol - 1) ? cols - 1 : (j + 1) * blockSizeCols - 1;

                    ranges[index] = (rowStart, rowEnd, colStart, colEnd);
                    index++;
                }
            }

            return ranges;
        }

        public static Matrix MultiplyMatrixUsingTreadsAndDividingMatrixIntoRanges(Matrix a, Matrix b, int threadsCount)
        {
            if (a.Cols != b.Rows)
            {
                throw new ArgumentException("Number of columns in the first matrix must be equal to the number of rows in the second matrix.");
            }

            Matrix result = new Matrix(a.Rows, b.Cols);
            var ranges = GetMatrixBlockRanges(result, threadsCount);
            Thread[] threadsList = new Thread[threadsCount];

            for (int i = 0; i < threadsCount; i++)
            {
                threadsList[i] = new Thread(ThreadMethodUsingTreadsAndDividingMatrixIntoRanges);
                threadsList[i].Start(new ThreadParametersUsingTreadsAndDividingMatrixIntoRanges
                {
                    RowStartIndex = ranges[i].Item1,
                    RowEndIndex = ranges[i].Item2,
                    ColumnStartIndex = ranges[i].Item3,
                    ColumnEndIndex = ranges[i].Item4,
                    ARef = a,
                    BRef = b,
                    ResultRef = result
                });
            }

            foreach (Thread thread in threadsList)
            {
                thread.Join();
            }

            return result;
        }
        #endregion MultiplyMatrixUsingTreadsAndDividingMatrixIntoRanges

        #region TaskMethodUsingTasks
        private class TaskParameters
        {
            public int RowStartIndex { get; set; }
            public int RowEndIndex { get; set; }
            public int ColumnStartIndex { get; set; }
            public int ColumnEndIndex { get; set; }
            public Matrix ARef { get; set; }
            public Matrix BRef { get; set; }
            public Matrix ResultRef { get; set; }
        }
        private static void TaskMethodUsingTasks(int rowStartIndex, int rowEndIndex, int columnStartIndex, int columnEndIndex, Matrix ARef, Matrix BRef, Matrix ResultRef)
        {


            for (int i = rowStartIndex; i <= rowEndIndex; i++)
            {
                for (int j = columnStartIndex; j <= columnEndIndex; j++)
                {
                    int sum = 0;
                    for (int k = 0; k < ARef.Cols; k++)
                    {
                        sum += ARef[i, k] * BRef[k, j];
                    }
                    ResultRef[i, j] = sum;
                }
            }
        }


        public static Matrix MultiplyParallelWithTasks(Matrix a, Matrix b, int tasksCount)
        {
            if (a.Cols != b.Rows)
            {
                throw new ArgumentException("Number of columns in the first matrix must be equal to the number of rows in the second matrix.");
            }

            Matrix result = new Matrix(a.Rows, b.Cols);
            var ranges = GetMatrixBlockRanges(result, tasksCount);
            var tasks = new Task[tasksCount];

            for (int i = 0; i < tasksCount; i++)
            {
                int rowStartIndex = ranges[i].Item1;
                int rowEndIndex = ranges[i].Item2;
                int columnStartIndex = ranges[i].Item3;
                int columnEndIndex = ranges[i].Item4;
                tasks[i] = Task.Run(() => TaskMethodUsingTasks(rowStartIndex, rowEndIndex, columnStartIndex, columnEndIndex, a, b, result));
            }

            Task.WaitAll(tasks);

            return result;
        }
        #endregion TaskMethodUsingTasks
    }
}
