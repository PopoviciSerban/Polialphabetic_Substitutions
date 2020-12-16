using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;

namespace Substitutii_polialfabetice
{
    class Program
    {
        static void Main(string[] args)
        {
            string s;
            string k;
            int key;
            
            Console.WriteLine("1. Vigenere Chiper\nGive the text");
            s = Console.ReadLine();

            Console.WriteLine("Give the key word");
            k = Console.ReadLine();

            VigenereChiper txt1 = new VigenereChiper(s, k);

            txt1.Encrypt();
            Console.WriteLine(@"Encrypted text: {0}", txt1.ToString());

            txt1.Decrypt();
            Console.WriteLine(@"Decrypted text: {0}", txt1.ToString());

            Console.WriteLine("\n2. FairPlay Chiper\nGive the text");
            s = Console.ReadLine();

            Console.WriteLine("Give the key word(s)");
            k = Console.ReadLine();

            PlayFairChiper txt2 = new PlayFairChiper(s, k);

            txt2.Encrypt();
            Console.WriteLine(@"Encrypted text: {0}", txt2.ToString());

            txt2.Decrypt();
            Console.WriteLine(@"Decrypted text: {0}", txt2.ToString());

            Console.WriteLine("\n3. Jefferson Cylinder\nGive the text (maximum 100 characters)");
            s = Console.ReadLine();

            Console.WriteLine("Give the key value");
            key = int.Parse(Console.ReadLine());

            JeffersonCylinder txt3 = new JeffersonCylinder(s, key);

            txt3.Encrypt();
            Console.WriteLine(@"Encrypted text: {0}", txt3.ToString());

            txt3.Decrypt();
            Console.WriteLine(@"Decrypted text: {0}", txt3.ToString());

            Console.ReadKey();
        }
    }

    class VigenereChiper
    {
        private string Text { get; set; }
        private string Key { get; set; }

        public VigenereChiper(string text, string key)
        {
            this.Text = text;
            this.Key = key;

            this.Text = this.Text.ToUpper(new CultureInfo("en-US", false));
            this.Key = this.Key.ToUpper(new CultureInfo("en-US", false));
        }

        public void Encrypt()
        {
            string output = string.Empty;
            int i = 0;

            foreach (char ch in this.Text)
            {
                if (!char.IsLetter(ch))
                    output += ch;
                else
                {
                    char d = 'A';
                    output += (char)(((ch + this.Key[i]) % 26) + d);
                }

                i = (i + 1) % this.Key.Length;
            }

            this.Text = output;
        }

        public void Decrypt()
        {
            string output = string.Empty;
            int i = 0;

            foreach (char ch in this.Text)
            {
                if (!char.IsLetter(ch))
                    output += ch;
                else
                {
                    char d = 'A';

                    if (this.Key[i] - d <= ch - d)
                        output += (char)(ch - this.Key[i] + d);
                    else
                        output += (char)(ch - this.Key[i] + d + 26);
                }

                i = (i + 1) % this.Key.Length;
            }

            this.Text = output;
        }

        public override string ToString() => this.Text;
    }

    class PlayFairChiper
    {
        private string Text;
        private char[,] Key= new char[5,5];
        private bool[] usedLetters = new bool[26];

        public PlayFairChiper(string text, string key)
        {
            this.Text = text.ToUpper(new CultureInfo("en-US", false));
            key = key.ToUpper(new CultureInfo("en-US", false));

            Queue<char> q = new Queue<char>();

            foreach(char c in key)
            {
                if (char.IsLetter(c) && !usedLetters[c - 'A'])
                {
                    usedLetters[c - 'A'] = true;
                    q.Enqueue(c);
                }
            }

            for (int i = 0; i <= 25; i++)
                if (!usedLetters[i])
                {
                    usedLetters[i] = true;
                    q.Enqueue((char)('A' + i));
                }

            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    if (q.Peek() != 'J')
                        this.Key[i, j] = q.Dequeue();
                    else
                    {
                        j--;
                        q.Dequeue();
                    }
        }

        private static void GetPosition(ref char[,] keySquare, char c, ref int row, ref int col)
        {
            if (c == 'J')
                GetPosition(ref keySquare, 'I', ref row, ref col);

            for (int i = 0; i < 5; ++i)
                for (int j = 0; j < 5; ++j)
                    if (c == keySquare[i, j])
                    {
                        row = i;
                        col = j;
                    }
        }

        private static string Chiper(string text, char[,] keySquare, int x)
        {
            string tempText = string.Empty;
            string output = string.Empty;
            int i;

            if (x == 1)
            {
                for (i = 0; i < text.Length; i++)
                    if (char.IsLetter(text[i]))
                    {
                        if (i > 0 && text[i] == text[i - 1])
                            tempText += 'X';

                        tempText += text[i];
                    }

                if (tempText.Length % 2 != 0)
                    tempText += 'X';
            }
            else
                tempText = text;


            for (i = 0; i < tempText.Length; i += 2)
            {
                int row1 = 0;
                int col1 = 0;
                int row2 = 0;
                int col2 = 0;

                GetPosition(ref keySquare, tempText[i], ref row1, ref col1);
                GetPosition(ref keySquare, tempText[i + 1], ref row2, ref col2);

                if (row1 == row2 && col1 == col2)
                {
                    output += keySquare[((row1 + x) % 5 + 5) % 5, ((col1 + x) % 5 + 5) % 5];
                    output += keySquare[((row1 + x) % 5 + 5) % 5, ((col1 + x) % 5 + 5) % 5];
                }
                else if (row1 == row2)
                {
                    output += keySquare[row1, ((col1 + x) % 5 + 5) % 5];
                    output += keySquare[row1, ((col2 + x) % 5 + 5) % 5];
                }
                else if (col1 == col2)
                {
                    output += keySquare[((row1 + x) % 5 + 5) % 5, col1];
                    output += keySquare[((row2 + x) % 5 + 5) % 5, col1];
                }
                else
                {
                    output += keySquare[row1, col2];
                    output += keySquare[row2, col1];
                }
            }

            return output;
        }

        public void Encrypt() => this.Text = Chiper(this.Text, this.Key, 1);

        public void Decrypt() => this.Text = Chiper(this.Text, this.Key, -1);

        public override string ToString() => this.Text;
    }

    class JeffersonCylinder
    {
        private int Key;
        private string Text;
        private int[,] Alphabet = new int[105, 27];

        public JeffersonCylinder(string text, int key)
        {
            this.Key = key;

            StringBuilder s = new StringBuilder();
            Random rand = new Random();
            int n = 0;

            foreach (char c in text)
                if (char.IsLetter(c)){
                    s.Append(c);
                    n++;
                }

            this.Text = s.ToString().ToUpper(new CultureInfo("en-US", false));

            for(int i = 0; i < n; i++)
            {
                bool[] used_letters = new bool[26];
                int j = 0;
                int k = 0;

                while (k != 26)
                {
                    int poz = rand.Next(0, 26);

                    if (!used_letters[poz])
                    {
                        used_letters[poz] = true;
                        Alphabet[i, j++] = poz;
                        k++;
                    }
                }
            }
        }

        public void Encrypt()
        {
            StringBuilder s = new StringBuilder();
            int i = 0;

            foreach(char c in this.Text)
            {
                for (int j = 0; j < 26; j++)
                    if ((char)('A' + Alphabet[i, j]) == c)
                        s.Append((char)('A' + Alphabet[i, (j + this.Key) % 26]));
 
                i++;
            }

            this.Text = s.ToString();
        }

        public void Decrypt()
        {
            StringBuilder s = new StringBuilder();
            int i = 0;

            foreach (char c in this.Text)
            {
                for (int j = 0; j < 26; j++)
                    if ((char)('A' + Alphabet[i, j]) == c)
                        s.Append((char)('A' + Alphabet[i, (j - this.Key + 26) % 26]));

                i++;
            }

            this.Text = s.ToString();
        }

        public override string ToString() => this.Text;
    }
}
