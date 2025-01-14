using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace MatrixCalculator
{
    public partial class MainCalcForm : Form
    {
        #region ControlDeclare
        private TextBox txtRowsMatrixFirst;
        private TextBox txtColumnsMatrixFirst;
        private TextBox txtRowsMatrixSecond;
        private TextBox txtColumnsMatrixSecond;
        private Button btnGenerateFirst;
        private Button btnGenerateSecond;
        private Button btnGenerateResult;

        private Panel matrixInputPanelFirst;
        private Panel matrixInputPanelSecond;
        private Panel matrixResultPanel;

        private TextBox[,] matrixInputsFirst;
        private TextBox[,] matrixInputsSecond;
        private TextBox[,] matrixInputsResult;

        private Label labelMatrixFirst;
        private Label labelMatrixSecond;
        private Label labelMatrixResultSeq;
        private Label labelMatrixResultPar;
        #endregion ControlDeclare

        #region FormUiComponentsInitialize
        public MainCalcForm()
        {
            InitializeComponent();
            GenerateMatrixLayout();
        }

        private void InitializeComponent()
        {
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1000, 300);
            this.MinimumSize = new Size(1000, 300);
            this.Name = "MainCalcForm";
            this.Text = "Matrix Calculator";
        }

        private void GenerateMatrixLayout()
        {
            // First Matrix Input Panel
            matrixInputPanelFirst = CreatePanel(DockStyle.Left, Color.Blue, this.ClientSize.Width / 3);

            var headerFirst = CreateHeaderPanel();
            txtRowsMatrixFirst = CreateTextBox(20, 5);
            txtRowsMatrixFirst.TextChanged += InputMatrixSizeTextBoxLimit;
            txtRowsMatrixFirst.KeyPress += new KeyPressEventHandler(TextBox_KeyPress);

            txtColumnsMatrixFirst = CreateTextBox(75, 5);
            txtColumnsMatrixFirst.TextChanged += FirtsInputMatrixColumnTextChanged;
            txtColumnsMatrixFirst.TextChanged += InputMatrixSizeTextBoxLimit;
            txtColumnsMatrixFirst.KeyPress += new KeyPressEventHandler(TextBox_KeyPress);

            btnGenerateFirst = CreateButton(135, 3, "Создать");
            btnGenerateFirst.Click += BtnGenerateFirst_Click;

            labelMatrixFirst = CreateLabel(235, 7 , "Матрица 1");

            headerFirst.Controls.AddRange(new Control[] { txtRowsMatrixFirst, txtColumnsMatrixFirst, btnGenerateFirst, labelMatrixFirst});

            matrixInputPanelFirst.Controls.Add(headerFirst);

            // Second Matrix Input Panel
            matrixInputPanelSecond = CreatePanel(DockStyle.Left, Color.Yellow, this.ClientSize.Width / 3);

            var headerSecond = CreateHeaderPanel();
            txtRowsMatrixSecond = CreateTextBox(20, 5);
            txtRowsMatrixSecond.TextChanged += SecondInputMatrixRowTextChanged;
            txtRowsMatrixSecond.TextChanged += InputMatrixSizeTextBoxLimit;
            txtRowsMatrixSecond.KeyPress += new KeyPressEventHandler(TextBox_KeyPress);

            txtColumnsMatrixSecond = CreateTextBox(75, 5);
            txtColumnsMatrixSecond.TextChanged += InputMatrixSizeTextBoxLimit;
            txtColumnsMatrixSecond.KeyPress += new KeyPressEventHandler(TextBox_KeyPress);

            btnGenerateSecond = CreateButton(135, 3, "Создать");
            btnGenerateSecond.Click += BtnGenerateSecond_Click;
            labelMatrixSecond = CreateLabel(235, 7, "Матрица 2");


            headerSecond.Controls.AddRange(new Control[] { txtRowsMatrixSecond, txtColumnsMatrixSecond, btnGenerateSecond, labelMatrixSecond});

            matrixInputPanelSecond.Controls.Add(headerSecond);

            // Result Panel
            matrixResultPanel = CreatePanel(DockStyle.Fill, Color.Orange);

            var headerResult = CreateHeaderPanel();
            btnGenerateResult = CreateButton(20, 5, "Умножить");
            btnGenerateResult.Enabled = false;
            btnGenerateResult.Click += BtnGenerateResult_Click;

            labelMatrixResultSeq = CreateLabel(120, 7, "");
            labelMatrixResultPar = CreateLabel(240, 7, "");


            headerResult.Controls.AddRange(new Control[] { btnGenerateResult, labelMatrixResultSeq, labelMatrixResultPar});

            matrixResultPanel.Controls.Add(headerResult);

            this.Controls.Add(matrixResultPanel);
            this.Controls.Add(matrixInputPanelSecond);
            this.Controls.Add(matrixInputPanelFirst);

            this.Resize += OnFormResize;
        }
        #region InputHandlers
        private void FirtsInputMatrixColumnTextChanged(object sender, EventArgs e)
        {
            txtRowsMatrixSecond.Text = txtColumnsMatrixFirst.Text;
        }
        private void SecondInputMatrixRowTextChanged(object sender, EventArgs e)
        {
            txtColumnsMatrixFirst.Text = txtRowsMatrixSecond.Text;
        }
        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true; 
            }
        }
        private void InputMatrixSizeTextBoxLimit(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (int.TryParse(textBox.Text, out int value))
                {
                    if (value > 30)
                    {
                        textBox.Text = "30";
                    }
                }
                else
                {
                    textBox.Text = "30";
                }
            }
        }
        #endregion FormUiComponentsInitialize

        #endregion InputHandlers

        #region ControlGenerators
        private Panel CreatePanel(DockStyle dock, Color color, int width = 0)
        {
            return new Panel
            {
                Dock = dock,
                //BackColor = color,
                Width = width
            };
        }
        private Label CreateLabel(int x, int y, string text)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),

            };
        }

        private Panel CreateHeaderPanel()
        {
            return new Panel
            {
                Dock = DockStyle.Top,
                Height = 30,
                //BackColor = Color.Gray
            };
        }

        private TextBox CreateTextBox(int x, int y)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(50, 20),
            };
        }

        private Button CreateButton(int x, int y, string text)
        {
            return new Button
            {
                Location = new Point(x, y),
                Size = new Size(100, 23),
                Text = text
                
            };
        }

        private void GenerateMatrix(Panel panel, ref TextBox[,] matrixInputs, TextBox txtRows, TextBox txtColumns, bool result = false)
        {


            // Clearing previous matrix grid
            for (int i = panel.Controls.Count - 1; i >= 1; i--)
            {
                panel.Controls.RemoveAt(i);
            }

            if (!int.TryParse(txtRows.Text, out int rows) || rows <= 0)
            {
                //txtRows.BackColor = Color.FromArgb(169, 93, 86);
                MessageBox.Show("Введите корректные размеры матрицы.\nРазмер матрицы не может быть отрицательным или нулевым.");
                return;
            }
            if (!int.TryParse(txtColumns.Text, out int columns) || columns <= 0)
            {
                //txtColumns.BackColor = Color.FromArgb(169, 93, 86);
                MessageBox.Show("Введите корректные размеры матрицы.\nРазмер матрицы не может быть отрицательным или нулевым.");
                return;
            }

            matrixInputs = new TextBox[rows, columns];
            int cellSize = Math.Min(panel.ClientSize.Width / columns, panel.ClientSize.Height / rows) - 5;

            int totalWidth = columns * cellSize;
            int totalHeight = rows * cellSize;

            int startX = (panel.ClientSize.Width - totalWidth) / 2;
            int startY = 40; 

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    var textBox = new TextBox
                    {
                        Size = new Size(cellSize, cellSize),
                        Location = new Point(startX + j * cellSize, startY + i * cellSize),
                        TextAlign = HorizontalAlignment.Center,
                        Text = "0"
                    };
                    if (result)
                    {
                        textBox.Enabled = false;
                    }
                    else
                    {
                        textBox.KeyPress += new KeyPressEventHandler(TextBox_KeyPress);
                    }
                    panel.Controls.Add(textBox);
                    matrixInputs[i, j] = textBox;
                }
            }
        }
        #endregion ControlGenerators

        #region ButtonClicks

        private void BtnGenerateFirst_Click(object sender, EventArgs e)
        {
            GenerateMatrix(matrixInputPanelFirst, ref matrixInputsFirst, txtRowsMatrixFirst, txtColumnsMatrixFirst);
            UpdateGenerateResultButtonState();
        }

        private void BtnGenerateSecond_Click(object sender, EventArgs e)
        {
            GenerateMatrix(matrixInputPanelSecond, ref matrixInputsSecond, txtRowsMatrixSecond, txtColumnsMatrixSecond);
            UpdateGenerateResultButtonState();
        }
        private Matrix ExtractMatrix(TextBox[,] textBoxes)
        {
            if (textBoxes == null || textBoxes.GetLength(0) == 0 || textBoxes.GetLength(1) == 0)
            {
                throw new ArgumentException("Передан некорректный массив.");
            }

            int rows = textBoxes.GetLength(0);
            int cols = textBoxes.GetLength(1);
            Matrix matrix = new Matrix(rows, cols);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (int.TryParse(textBoxes[i, j].Text, out int value))
                    {
                        matrix[i, j] = value;
                    }
                    else
                    {
                        throw new FormatException($"Некорректное значение в ячейке на позиции ({i}, {j}).");
                    }
                }
            }

            return matrix;
        }
        private void BtnGenerateResult_Click(object sender, EventArgs e)
        {

            if (matrixInputsFirst.GetLength(1) != matrixInputsSecond.GetLength(0))
            {
                MessageBox.Show("Такие матрицы нельзя перемножить, так как количество столбцов первой матрицы не равно количеству строк второй матрицы");
                return;
            }
            GenerateMatrix(matrixResultPanel, ref matrixInputsResult, txtRowsMatrixFirst, txtColumnsMatrixSecond, true);
            try
            {
                Matrix firstMatrix = ExtractMatrix(matrixInputsFirst);
                Matrix secondMatrix = ExtractMatrix(matrixInputsSecond);
                try
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    Matrix resultMatrix = Matrix.MultiplyMatrixSequential(firstMatrix, secondMatrix);
                    labelMatrixResultSeq.Text = $"Время пар: {stopwatch.ElapsedMilliseconds} ms";
                    InsertMatrix(textBoxMatrix: matrixInputsResult, resultMatrix: resultMatrix);
                    stopwatch.Restart();
                    Matrix.MultiplyParallelWithTasks(firstMatrix, secondMatrix, Environment.ProcessorCount);
                    labelMatrixResultPar.Text = $"Время пос: {stopwatch.ElapsedMilliseconds} ms";
                    stopwatch.Stop();

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обработке {ex.Message}");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обработке {ex.Message}");
                return;
            }

        }
        #endregion ButtonClicks

        #region SubMethods
        private void InsertMatrix(TextBox[,] textBoxMatrix, Matrix resultMatrix)
        {
            for (int i = 0; i < resultMatrix.Rows; i++)
            {
                for (int j = 0; j < resultMatrix.Cols; j++)
                {
                    textBoxMatrix[i, j].Text = resultMatrix[i, j].ToString();
                }
            }
        }
        private void UpdateGenerateResultButtonState()
        {
            btnGenerateResult.Enabled =
                int.TryParse(txtColumnsMatrixFirst.Text, out int columnsFirst) &&
                int.TryParse(txtRowsMatrixSecond.Text, out int rowsSecond) &&
                columnsFirst == rowsSecond &&
                matrixInputsFirst is not null &&
                matrixInputsSecond is not null;
        }
        #endregion SubMethods

        #region ResizeHandlers
        private void OnFormResize(object sender, EventArgs e)
        {
            matrixInputPanelFirst.Width = this.ClientSize.Width / 3;
            matrixInputPanelSecond.Width = this.ClientSize.Width / 3;

            if (matrixInputsFirst != null)
                GenerateMatrix(matrixInputPanelFirst, ref matrixInputsFirst, txtRowsMatrixFirst, txtColumnsMatrixFirst);

            if (matrixInputsSecond != null)
                GenerateMatrix(matrixInputPanelSecond, ref matrixInputsSecond, txtRowsMatrixSecond, txtColumnsMatrixSecond);

            if (matrixInputsResult != null)
                GenerateMatrix(matrixResultPanel, ref matrixInputsResult, txtRowsMatrixFirst, txtColumnsMatrixSecond, true);
        }
        #endregion ResizeHandlers

    }
}
