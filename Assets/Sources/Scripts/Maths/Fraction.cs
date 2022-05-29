using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using UnityEngine;

public class Fraction : Operation
{
    public int Numerator { get; private set; }
    public int Denominator { get; private set; }

    public override float Solution => ToFloat();

    public override string Definition => ToString();

    public Fraction(float value)
    {
        var temp = ToFraction(value);
        Initialize(temp.Numerator, temp.Denominator);
    }

    public Fraction(int value)
    {
        Initialize(value, 1);
    }

    public Fraction(int iNumerator, int iDenominator)
    {
        Initialize(iNumerator, iDenominator);
    }

    private void Initialize(int numerator, int denominator)
    {
        Numerator = numerator;
        Denominator = denominator;
        ReduceFraction(this);
    }

    public int Value
    {
        set
        {
            Numerator = value;
            Denominator = 1;
        }
    }

    public override string ToString()
    {
        return Denominator == 1 ? Numerator.ToString() : $"{Numerator}/{Denominator}";
    }

    public static Fraction ToFraction(float value)
    {
        if (value % 1 == 0)
        {
            return new Fraction((int)value);
        }

        var temp = value;
        var multiple = 1;
        var strTemp = value.ToString(CultureInfo.InvariantCulture);

        int i = 0;
        while (strTemp[i] != '.')
            i++;
        int iDigitsAfterDecimal = strTemp.Length - i - 1;

        while (iDigitsAfterDecimal > 0)
        {
            temp *= 10;
            multiple *= 10;
            iDigitsAfterDecimal--;
        }

        return new Fraction((int)Mathf.Round(temp), multiple);
    }

    public float ToFloat()
    {
        if (Denominator == 1)
        {
            return Numerator;
        }
        else
        {
            return Numerator / (float)Denominator;
        }
    }

    public static Fraction Inverse(Fraction frac1)
    {

        int iNumerator = frac1.Denominator;
        int iDenominator = frac1.Numerator;
        return (new Fraction(iNumerator, iDenominator));
    }

    public static Fraction operator -(Fraction frac1)
    { return (Negate(frac1)); }

    public static Fraction operator +(Fraction frac1, Fraction frac2)
    { return (Add(frac1, frac2)); }

    public static Fraction operator +(int iNo, Fraction frac1)
    { return (Add(frac1, new Fraction(iNo))); }

    public static Fraction operator +(Fraction frac1, int iNo)
    { return (Add(frac1, new Fraction(iNo))); }

    public static Fraction operator +(float dbl, Fraction frac1)
    { return (Add(frac1, Fraction.ToFraction(dbl))); }

    public static Fraction operator +(Fraction frac1, float dbl)
    { return (Add(frac1, Fraction.ToFraction(dbl))); }

    public static Fraction operator -(Fraction frac1, Fraction frac2)
    { return (Add(frac1, -frac2)); }

    public static Fraction operator -(int iNo, Fraction frac1)
    { return (Add(-frac1, new Fraction(iNo))); }

    public static Fraction operator -(Fraction frac1, int iNo)
    { return (Add(frac1, -(new Fraction(iNo)))); }

    public static Fraction operator -(float dbl, Fraction frac1)
    { return (Add(-frac1, Fraction.ToFraction(dbl))); }

    public static Fraction operator -(Fraction frac1, float dbl)
    { return (Add(frac1, -Fraction.ToFraction(dbl))); }

    public static Fraction operator *(Fraction frac1, Fraction frac2)
    { return (Multiply(frac1, frac2)); }

    public static Fraction operator *(int iNo, Fraction frac1)
    { return (Multiply(frac1, new Fraction(iNo))); }

    public static Fraction operator *(Fraction frac1, int iNo)
    { return (Multiply(frac1, new Fraction(iNo))); }

    public static Fraction operator *(float dbl, Fraction frac1)
    { return (Multiply(frac1, Fraction.ToFraction(dbl))); }

    public static Fraction operator *(Fraction frac1, float dbl)
    { return (Multiply(frac1, Fraction.ToFraction(dbl))); }

    public static Fraction operator /(Fraction frac1, Fraction frac2)
    { return (Multiply(frac1, Inverse(frac2))); }

    public static Fraction operator /(int iNo, Fraction frac1)
    { return (Multiply(Inverse(frac1), new Fraction(iNo))); }

    public static Fraction operator /(Fraction frac1, int iNo)
    { return (Multiply(frac1, Inverse(new Fraction(iNo)))); }

    public static Fraction operator /(float dbl, Fraction frac1)
    { return (Multiply(Inverse(frac1), Fraction.ToFraction(dbl))); }

    public static Fraction operator /(Fraction frac1, float dbl)
    { return (Multiply(frac1, Fraction.Inverse(Fraction.ToFraction(dbl)))); }

    public static bool operator ==(Fraction frac1, Fraction frac2)
    { return frac1 is { } && frac1.Equals(frac2); }

    public static bool operator !=(Fraction frac1, Fraction frac2)
    { return frac1 is { } && (!frac1.Equals(frac2)); }

    public static bool operator ==(Fraction frac1, int iNo)
    { return frac1 is { } && frac1.Equals(new Fraction(iNo)); }

    public static bool operator !=(Fraction frac1, int iNo)
    { return frac1 is { } && (!frac1.Equals(new Fraction(iNo))); }

    public static bool operator ==(Fraction frac1, float dbl)
    { return frac1 is { } && frac1.Equals(new Fraction(dbl)); }

    public static bool operator !=(Fraction frac1, float dbl)
    { return frac1 is { } && (!frac1.Equals(new Fraction(dbl))); }

    public static bool operator <(Fraction frac1, Fraction frac2)
    { return frac1.Numerator * frac2.Denominator < frac2.Numerator * frac1.Denominator; }

    public static bool operator >(Fraction frac1, Fraction frac2)
    { return frac1.Numerator * frac2.Denominator > frac2.Numerator * frac1.Denominator; }

    public static bool operator <=(Fraction frac1, Fraction frac2)
    { return frac1.Numerator * frac2.Denominator <= frac2.Numerator * frac1.Denominator; }

    public static bool operator >=(Fraction frac1, Fraction frac2)
    { return frac1.Numerator * frac2.Denominator >= frac2.Numerator * frac1.Denominator; }

    public static implicit operator Fraction(int lNo)
    { return new Fraction(lNo); }

    public static implicit operator Fraction(float dNo)
    { return new Fraction(dNo); }

    public static implicit operator string(Fraction frac)
    { return frac.ToString(); }

    private static Fraction Negate(Fraction frac1)
    {
        int iNumerator = -frac1.Numerator;
        int iDenominator = frac1.Denominator;
        return new Fraction(iNumerator, iDenominator);
    }

    private static Fraction Add(Fraction frac1, Fraction frac2)
    {
        int iNumerator = frac1.Numerator * frac2.Denominator + frac2.Numerator * frac1.Denominator;
        int iDenominator = frac1.Denominator * frac2.Denominator;
        return new Fraction(iNumerator, iDenominator);
    }

    private static Fraction Multiply(Fraction frac1, Fraction frac2)
    {
        int iNumerator = frac1.Numerator * frac2.Numerator;
        int iDenominator = frac1.Denominator * frac2.Denominator;
        return new Fraction(iNumerator, iDenominator);
    }

    private static int Gcd(int iNo1, int iNo2)
    {
        if (iNo1 < 0) iNo1 = -iNo1;
        if (iNo2 < 0) iNo2 = -iNo2;

        do
        {
            if (iNo1 < iNo2)
            {
                int tmp = iNo1;
                iNo1 = iNo2;
                iNo2 = tmp;
            }
            iNo1 %= iNo2;
        } while (iNo1 != 0);
        return iNo2;
    }

    public static void ReduceFraction(Fraction frac)
    {
        if (frac.Numerator == 0)
        {
            frac.Denominator = 1;
            return;
        }

        var gcd = Gcd(frac.Numerator, frac.Denominator);
        frac.Numerator /= gcd;
        frac.Denominator /= gcd;

        if (frac.Denominator >= 0) return;

        frac.Numerator *= -1;
        frac.Denominator *= -1;
    }

    public override bool Equals(object obj)
    {
        var frac = (Fraction)obj;
        return frac is { } && Numerator == frac.Numerator && Denominator == frac.Denominator;
    }

    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public override int GetHashCode()
    {
        return Convert.ToInt32((Numerator ^ Denominator) & 0xFFFFFFFF);
    }
}
