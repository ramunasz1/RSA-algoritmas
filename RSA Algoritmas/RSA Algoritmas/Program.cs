using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RSAEncryptionApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Specify the directory and file paths for saving the keys and encrypted message
            string directory = @"C:\Users\Azuolynas\Desktop\RSA Algoritmas";
            string publicKeyPath = Path.Combine(directory, "PublicKey.txt");
            string privateKeyPath = Path.Combine(directory, "PrivateKey.txt");
            string encryptedMessagePath = Path.Combine(directory, "EncryptedMessage.txt");

            // Generate RSA key pair
            using (var rsaProvider = new RSACryptoServiceProvider())
            {
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. Generate RSA key pair");
                Console.WriteLine("2. Encrypt a message");
                Console.WriteLine("3. Decrypt an encrypted message");
                Console.Write("Enter your choice (1, 2, or 3): ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        GenerateKeyPair(rsaProvider, publicKeyPath, privateKeyPath);
                        break;
                    case "2":
                        EncryptMessage(rsaProvider, publicKeyPath, encryptedMessagePath);
                        break;
                    case "3":
                        DecryptMessage(rsaProvider, privateKeyPath, encryptedMessagePath);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Exiting the program.");
                        return;
                }
            }

            Console.WriteLine("\nPress any key to exit.");
            Console.ReadKey();
        }

        static void GenerateKeyPair(RSACryptoServiceProvider rsaProvider, string publicKeyPath, string privateKeyPath)
        {
            // Generate RSA key pair with a key size of 2048 bits
            rsaProvider.KeySize = 2048;
            RSAParameters publicKey = rsaProvider.ExportParameters(false);
            RSAParameters privateKey = rsaProvider.ExportParameters(true);

            // Convert the key parameters to XML string format
            string publicKeyXml = rsaProvider.ToXmlString(false);
            string privateKeyXml = rsaProvider.ToXmlString(true);

            Console.WriteLine("\nPublic key:\n" + publicKeyXml);

            // Save the public key to a file
            Directory.CreateDirectory(Path.GetDirectoryName(publicKeyPath));
            File.WriteAllText(publicKeyPath, publicKeyXml);
            Console.WriteLine($"\nPublic key has been saved to {publicKeyPath}");

            // Save the private key to a file
            Directory.CreateDirectory(Path.GetDirectoryName(privateKeyPath));
            File.WriteAllText(privateKeyPath, privateKeyXml);
            Console.WriteLine($"\nPrivate key has been saved to {privateKeyPath}");
        }

        static void EncryptMessage(RSACryptoServiceProvider rsaProvider, string publicKeyPath, string encryptedMessagePath)
        {
            // Check if the public key file exists
            if (!File.Exists(publicKeyPath))
            {
                Console.WriteLine("Public key file not found. Generate RSA key pair first. Exiting the program.");
                return;
            }

            // Read the public key from file
            string publicKeyXml = File.ReadAllText(publicKeyPath);
            rsaProvider.FromXmlString(publicKeyXml);

            // Encrypt a message
            Console.Write("\nEnter the message to encrypt: ");
            string message = Console.ReadLine();
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(message);
            byte[] encryptedBytes = rsaProvider.Encrypt(plaintextBytes, true);
            string encryptedText = Convert.ToBase64String(encryptedBytes);
            Console.WriteLine("Encrypted message:\n" + encryptedText);

            // Save the encrypted message to a file
            Directory.CreateDirectory(Path.GetDirectoryName(encryptedMessagePath));
            File.WriteAllText(encryptedMessagePath, encryptedText);
            Console.WriteLine($"\nEncrypted message has been saved to {encryptedMessagePath}");
        }

        static void DecryptMessage(RSACryptoServiceProvider rsaProvider, string privateKeyPath, string encryptedMessagePath)
        {
            // Check if the private key file exists
            if (!File.Exists(privateKeyPath))
            {
                Console.WriteLine("Private key file not found. Generate RSA key pair first. Exiting the program.");
                return;
            }

            // Read the private key from file
            string privateKeyXml = File.ReadAllText(privateKeyPath);
            rsaProvider.FromXmlString(privateKeyXml);

            // Check if the encrypted message file exists
            if (!File.Exists(encryptedMessagePath))
            {
                Console.WriteLine("Encrypted message file not found. Exiting the program.");
                return;
            }

            // Read the encrypted message from file
            string encryptedText = File.ReadAllText(encryptedMessagePath);

            // Decrypt the message
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] decryptedBytes = rsaProvider.Decrypt(encryptedBytes, true);
            string decryptedText = Encoding.UTF8.GetString(decryptedBytes);
            Console.WriteLine("\nDecrypted message:\n" + decryptedText);
        }
    }
}
