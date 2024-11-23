using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

public partial class LoginForm : Form
{
    private TextBox textBoxEmail;
    private TextBox textBoxPassword;
    private Button buttonLogin;
    private Label labelEmail;
    private Label labelPassword;
    private Label labelTitle;
    public bool IsLoggedIn { get; set; }

    public LoginForm()
    {
        this.Size = new System.Drawing.Size(400, 300);
        this.Text = "Вход в систему";
        this.BackColor = System.Drawing.Color.White;

        labelTitle = new Label();
        labelTitle.Text = "Вход в систему";
        labelTitle.Font = new System.Drawing.Font("Arial", 18, System.Drawing.FontStyle.Bold);
        labelTitle.Location = new System.Drawing.Point(10, 10);
        this.Controls.Add(labelTitle);

        labelEmail = new Label();
        labelEmail.Text = "Email:";
        labelEmail.Font = new System.Drawing.Font("Arial", 12);
        labelEmail.Location = new System.Drawing.Point(10, 150);
        this.Controls.Add(labelEmail);

        textBoxEmail = new TextBox();
        textBoxEmail.Location = new System.Drawing.Point(10, 170);
        textBoxEmail.Size = new System.Drawing.Size(200, 25);
        this.Controls.Add(textBoxEmail);

        labelPassword = new Label();
        labelPassword.Text = "Пароль:";
        labelPassword.Font = new System.Drawing.Font("Arial", 12);
        labelPassword.Location = new System.Drawing.Point(10, 200);
        this.Controls.Add(labelPassword);

        textBoxPassword = new TextBox();
        textBoxPassword.Location = new System.Drawing.Point(10, 220);
        textBoxPassword.Size = new System.Drawing.Size(200, 25);
        textBoxPassword.PasswordChar = '*';
        this.Controls.Add(textBoxPassword);

        buttonLogin = new Button();
        buttonLogin.Text = "Войти";
        buttonLogin.Font = new System.Drawing.Font("Arial", 12);
        buttonLogin.Location = new System.Drawing.Point(10, 250);
        buttonLogin.Click += new EventHandler(LoginButton_Click);
        this.Controls.Add(buttonLogin);

        this.Controls.Add(labelEmail);
        this.Controls.Add(textBoxEmail);
        this.Controls.Add(labelPassword);
        this.Controls.Add(textBoxPassword);
        this.Controls.Add(buttonLogin);
    }

    private void LoginButton_Click(object sender, EventArgs e)
    {
        string email = textBoxEmail.Text;
        string password = textBoxPassword.Text;

        // Проверяем, что поля не пустые
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            MessageBox.Show("Пожалуйста, заполните все поля!");
            return;
        }

        // Проверяем корректность email и пароля
        if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
        {
            MessageBox.Show("Неправильный email!");
            return;
        }
        if (password.Length < 8)
        {
            MessageBox.Show("Пароль должен быть не менее 8 символов!");
            return;
        }

        // Генерируем токен для защиты от атак по CSRF
        string token = Guid.NewGuid().ToString();

        // Отправляем запрос на сервер
        string url = "http://example.com/login.php";
        string data = $"email={email}&password={password}&token={token}";

        try
        {
            // Создаем запрос
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(data);
            }

            // Получаем ответ
            WebResponse response = request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string result = reader.ReadToEnd();

                // Проверяем, что вход был успешен
                if (result == "true")
                {
                    // Вход успешен
                    MessageBox.Show("Вы успешно вошли в систему!");
                    IsLoggedIn = true;
                }
                else
                {
                    // Вход не удался
                    MessageBox.Show("Неправильный email или пароль!");
                    IsLoggedIn = false;
                }
            }
        }
        catch (WebException ex)
        {
            // Обрабатываем исключения
            MessageBox.Show("Ошибка соединения с сервером!");
            IsLoggedIn = false;
        }
        catch (Exception ex)
        {
            // Обрабатываем другие исключения
            MessageBox.Show("Ошибка входа!");
            IsLoggedIn = false;
        }
    }

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new LoginForm());
    }
}