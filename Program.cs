/*Создать консольное приложение на C#, которое позволит пользователям управлять своими банковскими счетами.

Описание:

Команда должна состоять из 2-3 человек.
Создайте публичный репозиторий на GitHub для вашего проекта.
Разработайте класс BankAccount, который будет представлять банковский счет с полями: номер счета, баланс и владелец счета.
Создайте класс Bank, который будет отвечать за управление банковскими счетами: открытие новых счетов, пополнение счетов, списание средств, переводы между счетами и вывод информации о счетах.
Реализуйте консольный интерфейс для взаимодействия с пользователями. Приложение должно предоставлять следующие команды:
"Создать счет" - создание нового банковского счета с указанием номера счета и имени владельца.
"Пополнить счет" - пополнение баланса счета по его номеру.
"Снять со счета" - списание средств со счета по его номеру.
"Перевести средства" - перевод денег с одного счета на другой с указанием номеров счетов.
"Показать информацию о счете" - вывод информации о банковском счете по его номеру.
"Выход" - завершение работы приложения.
Каждое изменение в состоянии банковских счетов должно быть сохранено в файле (например, в формате JSON) с использованием библиотеки для работы с файлами, такой как System.IO.
При запуске приложения, оно должно загружать ранее сохраненное состояние счетов из файла (если такой файл есть) и предоставлять возможность продолжить работу с ними.
Бонусные задания:

Реализуйте возможность удаления счетов.
Добавьте проверку на недостаточность средств при списании.
Обеспечьте валидацию вводимых данных пользователя и информативные сообщения об ошибках.*/

using System.Text.Json;
class BankAccount
{
    public int Number { get; set; }
    public int Balance { get; set; }
    public string Owner { get; set; }

    public BankAccount()
    {
        Balance = 0;
    }

    public BankAccount(int number, int balance, string owner)
    {
        Number = number;
        Balance = balance;
        Owner = owner;
    }
}

class Bank
{
    private List<BankAccount> accounts;
    private string dataFilePath = "bank_data.json";

    public Bank()
    {
        accounts = new List<BankAccount>();
        LoadDataFromFile();
    }

    public void OpenAccount(int accountNumber, int accountBalance, string accountOwner)
    {
        if (accounts.Any(acc => acc.Number == accountNumber))
        {
            Console.WriteLine($"Счет с номером {accountNumber} уже существует.");
            return;
        }

        var newAccount = new BankAccount(accountNumber, accountBalance, accountOwner);
        accounts.Add(newAccount);
        Console.WriteLine($"Открыт новый счет для {accountOwner} с номером {accountNumber} и начальным балансом {accountBalance} .");
    }

    public void DisplayAccountInfo(int accountNumber)
    {
        var account = FindAccount(accountNumber);

        if (account != default)
        {
            Console.WriteLine($"Информация о счете {account.Number}:");
            Console.WriteLine($"Владелец счета: {account.Owner}");
            Console.WriteLine($"Баланс: {account.Balance} гривен.");
        }
        else
        {
            Console.WriteLine($"Счет с номером {accountNumber} не найден.");
        }
    }
    public void Deposit(int accountNumber, int amount)
    {
        var account = FindAccount(accountNumber);

        if (account == default)
        {
            Console.WriteLine($"Счет с номером {accountNumber} не найден.");
            return;
        }

        if (amount <= 0)
        {
            Console.WriteLine("Сумма для пополнения должна быть больше нуля.");
            return;
        }

        account.Balance += amount;
        Console.WriteLine($"Счет {accountNumber} успешно пополнен на {amount} гривен. Новый баланс: {account.Balance} гривен.");
    }

    public void Withdraw(int accountNumber, int amount)
    {
        var account = FindAccount(accountNumber);

        if (account == default)
        {
            Console.WriteLine($"Счет с номером {accountNumber} не найден.");
            return;
        }

        if (amount <= 0)
        {
            Console.WriteLine("Сумма для списания должна быть больше нуля.");
            return;
        }

        if (account.Balance < amount)
        {
            Console.WriteLine("Недостаточно средств на счете.");
            return;
        }

        account.Balance -= amount;
        Console.WriteLine($"Со счета {accountNumber} успешно списано {amount} гривен. Новый баланс: {account.Balance} гривен.");
    }

    public void Transfer(int sourceAccountNumber, int destinationAccountNumber, int amount)
    {
        var sourceAccount = FindAccount(sourceAccountNumber);
        var destinationAccount = FindAccount(destinationAccountNumber);

        if (sourceAccount == default || destinationAccount == default)
        {
            Console.WriteLine("Один из счетов не найден.");
            return;
        }

        if (amount <= 0)
        {
            Console.WriteLine("Сумма для перевода должна быть больше нуля.");
            return;
        }

        if (sourceAccount.Balance < amount)
        {
            Console.WriteLine("Недостаточно средств на счете для перевода.");
            return;
        }

        sourceAccount.Balance -= amount;
        destinationAccount.Balance += amount;

        Console.WriteLine($"Со счета {sourceAccountNumber} переведено {amount} гривен на счет {destinationAccountNumber}.");
        Console.WriteLine($"Новый баланс счета {sourceAccountNumber}: {sourceAccount.Balance} гривен.");
        Console.WriteLine($"Новый баланс счета {destinationAccountNumber}: {destinationAccount.Balance} гривен.");
    }
    private void SaveDataToFile()
    {
        try
        {
            string jsonData = JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(dataFilePath, jsonData);
            Console.WriteLine("Данные успешно сохранены в файл.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении данных в файл: {ex.Message}");
        }
    }
    private void LoadDataFromFile()
    {
        try
        {
            if (File.Exists(dataFilePath))
            {
                string jsonData = File.ReadAllText(dataFilePath);
                accounts = JsonSerializer.Deserialize<List<BankAccount>>(jsonData);
                Console.WriteLine("Данные успешно загружены из файла.");
            }
            else
            {
                Console.WriteLine("Файл данных не найден. Создан новый список счетов.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке данных из файла: {ex.Message}");
        }
    }
    public void CloseBank()
    {
        SaveDataToFile();
        Console.WriteLine("Банк закрыт. Данные сохранены.");
    }
    public BankAccount FindAccount(int accountNumber)
    {
        return accounts.FirstOrDefault(acc => acc.Number == accountNumber);
    }
}
class Program
{
    static void Main()
    {
        Bank bank = new Bank();

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Открыть новый счет");
            Console.WriteLine("2. Вывести информацию о счете");
            Console.WriteLine("3. Пополнить счет");
            Console.WriteLine("4. Списать средства со счета");
            Console.WriteLine("5. Перевести средства между счетами");
            Console.WriteLine("6. Выход");
            Console.Write("Ваш выбор: ");
            int choice;
            if (!int.TryParse(Console.ReadLine(), out choice))
            {
                Console.WriteLine("Неверный ввод. Попробуйте еще раз.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    Console.WriteLine("Введите номер нового счета:");
                    int accountNumber = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Введите имя владельца счета:");
                    string accountOwner = Console.ReadLine();
                    Console.WriteLine("Введите начальный баланс:");
                    if (!int.TryParse(Console.ReadLine(), out int initialBalance))
                    {
                        Console.WriteLine("Некорректный ввод суммы. Счет не будет создан.");
                        continue;
                    }
                    bank.OpenAccount(accountNumber, initialBalance, accountOwner);
                    break;

                case 2:
                    Console.WriteLine("Введите номер счета:");
                    int accountNumberInfo = Convert.ToInt32(Console.ReadLine());
                    bank.DisplayAccountInfo(accountNumberInfo);
                    break;

                case 3:
                    Console.WriteLine("Введите номер счета для пополнения:");
                    int depositAccountNumber = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Введите сумму для пополнения:");
                    if (!int.TryParse(Console.ReadLine(), out int depositAmount))
                    {
                        Console.WriteLine("Некорректный ввод суммы. Пополнение не выполнено.");
                        continue;
                    }
                    bank.Deposit(depositAccountNumber, depositAmount);
                    break;

                case 4:
                    Console.WriteLine("Введите номер счета для списания:");
                    int withdrawAccountNumber = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Введите сумму для списания:");
                    if (!int.TryParse(Console.ReadLine(), out int withdrawAmount))
                    {
                        Console.WriteLine("Некорректный ввод суммы. Списание не выполнено.");
                        continue;
                    }
                    bank.Withdraw(withdrawAccountNumber, withdrawAmount);
                    break;

                case 5:
                    Console.WriteLine("Введите номер счета-источника:");
                    int sourceAccountNumber = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Введите номер счета-получателя:");
                    int destinationAccountNumber = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Введите сумму для перевода:");
                    if (!int.TryParse(Console.ReadLine(), out int transferAmount))
                    {
                        Console.WriteLine("Некорректный ввод суммы. Перевод не выполнен.");
                        continue;
                    }
                    bank.Transfer(sourceAccountNumber, destinationAccountNumber, transferAmount);
                    break;

                case 6:
                    bank.CloseBank();
                    Console.WriteLine("Выход из программы.");
                    return;

                default:
                    Console.WriteLine("Неверный выбор. Попробуйте еще раз.");
                    break;
            }
        }
    }
}


