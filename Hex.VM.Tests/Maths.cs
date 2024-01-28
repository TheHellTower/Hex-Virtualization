namespace Hex.VM.Tests
{
    public class Maths
    {
        public int Sum { get; set; }
        private int _x, _y;
        
        public Maths(int x, int y)
        {
            _x = x;
            _y = y;
            Sum = _x + _y;
        }

        public int Add() => _x + _y;
        
        public int Subtract() => _x - _y;

        public int Multiply() => _x * _y;

        public int Divide() => _x / _y;
    }
}