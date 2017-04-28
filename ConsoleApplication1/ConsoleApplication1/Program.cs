using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            const string DOG_DATA_FILE = "DogData.dat";

            // Insantiate the Dog and give it values
            Dog.GetSingletonInstance().m_lfHeight = 100.5;
            Dog.GetSingletonInstance().m_nNumHairs = 100001;
            Console.WriteLine(Dog.GetSingletonInstance());

            // Serialize Dog
            Dog.GetSingletonInstance().Serialize(DOG_DATA_FILE);

            // Change Dog values to validate deserialize saved state
            Dog.GetSingletonInstance().m_lfHeight = 10;
            Dog.GetSingletonInstance().m_nNumHairs = 1;
            Console.WriteLine(Dog.GetSingletonInstance());

            // Deserialize Dog to restore state
            Dog.GetSingletonInstance().Deserialize(DOG_DATA_FILE);
            Console.WriteLine(Dog.GetSingletonInstance());

            Console.ReadKey();
        }
    }

    [Serializable()]
    class Dog
    {
        private static Dog m_SingletonDog;

        public double m_lfHeight;
        public int m_nNumHairs;

        private Dog()
        {
        }

        public override string ToString()
        {
            return "Dog: {Num Hairs: " + m_nNumHairs + ", Height: " + m_lfHeight + "}";
        }
        
        // Here's the Singleton Bread and Butter
        public static Dog GetSingletonInstance()
        {
            if (m_SingletonDog == null)
            {
                m_SingletonDog = new Dog();
            }

            return m_SingletonDog;
        }

        public void Serialize(string fileName)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();

            try
            {
                formatter.Serialize(fileStream, this);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fileStream.Close();
            }
        }

        public void Deserialize(string fileName)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                // Deserialize the hashtable from the file and 
                // assign the reference to the local variable.
                m_SingletonDog = (Dog) formatter.Deserialize(fileStream);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fileStream.Close();
            }
        }
    }
}
