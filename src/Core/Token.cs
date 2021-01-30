namespace Bannerlord.UIEditor.Core
{
    public readonly struct Token
    {
        #region Public Constants Statics

        public static bool operator ==(Token _token1, Token _token2) => _token1.m_Value == _token2.m_Value;

        public static bool operator !=(Token _token1, Token _token2) => _token1.m_Value != _token2.m_Value;

        public static Token Create() => new(m_NextValue++);

        public static Token Create(int _value) => new(_value);

        public static Token CreateInvalid() => Create(0);

        #endregion

        #region Static/Const Fields

        private static int m_NextValue = 1;

        #endregion

        #region Private Fields

        private readonly int m_Value;

        #endregion

        #region Constructors

        private Token(int _value) => m_Value = _value;

        #endregion

        #region ValueType Members

        public override bool Equals(object _obj) => _obj is Token other && Equals(other);

        public override int GetHashCode() => m_Value;

        #endregion

        #region Public Methods

        public bool Equals(Token _other) => m_Value == _other.m_Value;

        public bool IsValid() => m_Value != 0;

        #endregion
    }
}
