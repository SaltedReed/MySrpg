

namespace MyUtility
{

    public class Singleton<T> where T : Singleton<T>, new()
    {
        public static T Instance
        {
            get
            {
                if (m_instance is null)
                    m_instance = new T();
                return m_instance;
            }
        }
        private static T m_instance;

    }

}