namespace Bannerlord.UIEditor.Core
{
    public readonly struct Token
    {
        public static bool operator ==(Token _token1, Token _token2) => _token1.m_Value == _token2.m_Value;

        public static bool operator !=(Token _token1, Token _token2) => _token1.m_Value != _token2.m_Value;

        public static Token Create() => new(m_NextValue++);

        public static Token Create(int _value) => new(_value);

        public static Token CreateInvalid() => Create(0);

        private static int m_NextValue = 1;

        private readonly int m_Value;

        private Token(int _value) => m_Value = _value;

        public override bool Equals(object? _obj) => _obj is Token other && Equals(other);

        public override int GetHashCode() => m_Value;

        public bool Equals(Token _other) => m_Value == _other.m_Value;

        public bool IsValid() => m_Value != 0;
    }
}
