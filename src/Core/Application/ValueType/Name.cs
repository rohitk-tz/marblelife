namespace Core.Application.ValueType
{
    public class Name
    {
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string FullName { get { return ToString(); } }

        public Name(string firstName, string lastName)
            : this(firstName, string.Empty, lastName)
        {

        }

        public Name(string firstName, string middleName, string lastName)
        {
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
        }

        public override string ToString()
        {
            var name = !string.IsNullOrEmpty(MiddleName)
                       ? string.Format("{0} {1} {2}", FirstName, MiddleName, LastName)
                       : string.Format("{0} {1}", FirstName, LastName);

            return name;
        }

        public Name()
        {

        }
    }
}
