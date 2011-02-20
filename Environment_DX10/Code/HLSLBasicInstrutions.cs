﻿using System;
using System.Collections.Generic;
using System.Text;
using Core.Basic;
using Core.CodeGeneration;
using Core.CodeGeneration.Code;
using System.Diagnostics;
using System.Globalization;
using Core.Var;

namespace Environment_DX10.Code
{
    public class HLSLBasicInstrutions : BasicInstructions
    {
        public string TranslateParameter(Variable d)
        {
            switch (d.Format)
            {
                case Format.TEXTURE: return string.Format("texture2D {0};\n", d.Name);
                case Format.SAMPLER: return string.Format(
                   "SamplerState {0}\n{{\n" +
                   "\tFilter = MIN_MAG_MIP_LINEAR;\n" +
                   "\tAddressU = Wrap;\n" +
                   "\tAddressV = Wrap;\n" +
                   "}};\n", d.Name);
                case Format.FLOAT: return string.Format("float {0};\n", d.Name);
                case Format.FLOAT2: return string.Format("float2 {0};\n", d.Name);
                case Format.FLOAT3: return string.Format("float3 {0};\n", d.Name);
                case Format.FLOAT4: return string.Format("float4 {0};\n", d.Name);
                case Format.FLOAT4X4: return string.Format("float4x4 {0};\n", d.Name);
            }

            throw new NotImplementedException();
        }

        public override string Translate(Core.CodeGeneration.Code.Instruction i)
        {
            if (i is CreateVariableInstruction)
                return Translate((CreateVariableInstruction)i);

            if (i is IfInstruction)
                return Translate((IfInstruction)i);

            throw new NotImplementedException();
        }

        string Translate(CreateVariableInstruction i)
        {
            VariableExpression l = (VariableExpression)i.AssignExpression.LeftExpression;

            if (i.AssignOnly)
                return string.Format("{0} = {1};\n", l.Variable.Name, Translate(i.AssignExpression.RightExpression));
            else
                return string.Format("{0} {1} = {2};\n", FormatToString(l.Variable.Format), l.Variable.Name, Translate(i.AssignExpression.RightExpression));
        }

        string Translate(IfInstruction i)
        {
            string s = "";

            if (i.DefinedOutputVariable != null)
                s += string.Format("{0} {1};\n\t", FormatToString(i.DefinedOutputVariable.Format), i.DefinedOutputVariable.Name);
            s += string.Format("if({0})\n\t\t{1}", Translate(i.Condition), Translate(i.IfTrue));
            if (i.IfFalse != null) 
                s += "\telse\n\t\t" + Translate(i.IfFalse);

            return s;
        }

        string Translate(Expression e)
        {
            if (e is ConstExpression)
                return Translate((ConstExpression)e);

            if (e is VariableExpression)
                return Translate((VariableExpression)e);

            if (e is UnaryExpression)
                return Translate((UnaryExpression)e);

            if (e is BinaryExpression)
                return Translate((BinaryExpression)e);

            if (e is CallExpression)
                return Translate((CallExpression)e);

            if (e is SwizzleExpression)
                return Translate((SwizzleExpression)e);

            if (e is VectorConstructorExpression)
                return Translate((VectorConstructorExpression)e);

            throw new NotImplementedException();
        }

        string Translate(ConstExpression e)
        {
            if (e.Value is Vector1f)
                return ((Vector1f)e.Value).X.ToString(CultureInfo.InvariantCulture);

            if (e.Value is Vector2f)
            {
                Vector2f v = (Vector2f)e.Value;
                return string.Format("float2({0}, {1})", v.X.ToString(CultureInfo.InvariantCulture), v.Y.ToString(CultureInfo.InvariantCulture));
            }

            if (e.Value is Vector3f)
            {
                Vector3f v = (Vector3f)e.Value;
                return string.Format("float3({0}, {1}, {2})", v.X.ToString(CultureInfo.InvariantCulture), v.Y.ToString(CultureInfo.InvariantCulture), v.Z.ToString(CultureInfo.InvariantCulture));
            }

            if (e.Value is Vector4f)
            {
                Vector4f v = (Vector4f)e.Value;
                return string.Format("float4({0}, {1}, {2}, {3})", v.X.ToString(CultureInfo.InvariantCulture), v.Y.ToString(CultureInfo.InvariantCulture), v.Z.ToString(CultureInfo.InvariantCulture), v.W.ToString(CultureInfo.InvariantCulture));
            }

            if (e.Value is Matrix44f)
            {
                Matrix44f m = (Matrix44f)e.Value;
                return string.Format("float4x4({0},{1},{2},{3}, {4},{5},{6},{7}, {8},{9},{10},{11}, {12},{13},{14},{15})",
                    m.Column0.X.ToString(CultureInfo.InvariantCulture), m.Column0.Y.ToString(CultureInfo.InvariantCulture), m.Column0.Z.ToString(CultureInfo.InvariantCulture), m.Column0.W.ToString(CultureInfo.InvariantCulture),
                    m.Column1.X.ToString(CultureInfo.InvariantCulture), m.Column1.Y.ToString(CultureInfo.InvariantCulture), m.Column1.Z.ToString(CultureInfo.InvariantCulture), m.Column1.W.ToString(CultureInfo.InvariantCulture),
                    m.Column2.X.ToString(CultureInfo.InvariantCulture), m.Column2.Y.ToString(CultureInfo.InvariantCulture), m.Column2.Z.ToString(CultureInfo.InvariantCulture), m.Column2.W.ToString(CultureInfo.InvariantCulture),
                    m.Column3.X.ToString(CultureInfo.InvariantCulture), m.Column3.Y.ToString(CultureInfo.InvariantCulture), m.Column3.Z.ToString(CultureInfo.InvariantCulture), m.Column3.W.ToString(CultureInfo.InvariantCulture));
            }

            throw new NotImplementedException();
        }

        string Translate(VariableExpression e)
        {
            return e.Variable.Name;
        }

        string Translate(UnaryExpression e)
        {
            switch (e.Operator)
            {
                case UnaryExpression.Operators.Minus: return "-" + Translate(e.Expression);
            }

            throw new NotImplementedException();
        }

        string Translate(BinaryExpression e)
        {
            switch (e.Operator)
            {
                case BinaryExpression.Operators.Add: return "(" + Translate(e.LeftExpression) + " + " + Translate(e.RightExpression) + ")";
                case BinaryExpression.Operators.Sub: return "(" + Translate(e.LeftExpression) + " - " + Translate(e.RightExpression) + ")";
                case BinaryExpression.Operators.Mul: return "(" + Translate(e.LeftExpression) + " * " + Translate(e.RightExpression) + ")";
                case BinaryExpression.Operators.Div: return "(" + Translate(e.LeftExpression) + " / " + Translate(e.RightExpression) + ")";

                case BinaryExpression.Operators.Equal: return "(" + Translate(e.LeftExpression) + " == " + Translate(e.RightExpression) + ")";
                case BinaryExpression.Operators.Less: return "(" + Translate(e.LeftExpression) + " < " + Translate(e.RightExpression) + ")";
            }

            throw new NotImplementedException();
        }

        string Translate(CallExpression e)
        {
            switch (e.FunctionType)
            {
                case CallExpression.Function.SampleTexture2D: return string.Format("{1}.Sample({0}, {2})", Translate(e.Parameters[0]), Translate(e.Parameters[1]), Translate(e.Parameters[2]));
                case CallExpression.Function.Sin: return string.Format("sin({0})", Translate(e.Parameters[0]));
                case CallExpression.Function.Cos: return string.Format("cos({0})", Translate(e.Parameters[0]));
                case CallExpression.Function.Dot: return string.Format("dot({0}, {1})", Translate(e.Parameters[0]), Translate(e.Parameters[1]));
                case CallExpression.Function.PositionTransform: return string.Format("mul({1}, {0})", Translate(e.Parameters[0]), Translate(e.Parameters[1]));
                case CallExpression.Function.Cross: return string.Format("cross({0}, {1})", Translate(e.Parameters[0]), Translate(e.Parameters[1]));
                case CallExpression.Function.Normalize : return string.Format("normalize({0})", Translate(e.Parameters[0]));
                case CallExpression.Function.Abs: return string.Format("abs({0})", Translate(e.Parameters[0]));
                case CallExpression.Function.Clamp: return string.Format("clamp({0}, {1}, {2})", Translate(e.Parameters[0]), Translate(e.Parameters[1]), Translate(e.Parameters[2]));
                case CallExpression.Function.Lerp: return string.Format("lerp({0}, {1}, {2})", Translate(e.Parameters[0]), Translate(e.Parameters[1]), Translate(e.Parameters[2]));
                case CallExpression.Function.Length: return string.Format("length({0})", Translate(e.Parameters[0]));
                case CallExpression.Function.Pow: return string.Format("pow({0}, {1})", Translate(e.Parameters[0]), Translate(e.Parameters[1]));
            }

            throw new NotImplementedException();
        }

        string Translate(SwizzleExpression e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Translate(e.Expression)).Append(".");

            foreach (var s in e.Swizzle)
            {
                switch (s)
                {
                    case VectorMembers.X: sb.Append("x"); break;
                    case VectorMembers.Y: sb.Append("y"); break;
                    case VectorMembers.Z: sb.Append("z"); break;
                    case VectorMembers.W: sb.Append("w"); break;
                }
            }

            return sb.ToString();
        }

        string Translate(VectorConstructorExpression e)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(FormatToString(e.Format)).Append('(');

            for (int i = 0; i < e.Values.Length; i++)
            {
                sb.Append(Translate(e.Values[i]));
                if (i + 1 != e.Values.Length)
                    sb.Append(", ");
                else
                    sb.Append(")");
            }

            return sb.ToString();
        }

        string FormatToString(Format f)
        {
            switch (f)
            {
                case Format.FLOAT: return "float";
                case Format.FLOAT2: return "float2";
                case Format.FLOAT3: return "float3";
                case Format.FLOAT4: return "float4";
            }

            throw new NotImplementedException();
        }

        public string GenerateCode(ShaderCode sc)
        {
            StringBuilder sb = new StringBuilder();

            DefineParameters(sc, sb);

            DefineStructs(sc, sb);

            DefineVertexShader(sc, sb);

            DefinePixelShader(sc, sb);

            DefineTechnique(sc, sb);

            return sb.ToString();
        }

        //------------

        void DefineParameters(ShaderCode sc, StringBuilder sb)
        {
            sb.Append("\n");

            foreach (var d in sc.Parameters)
                sb.Append(TranslateParameter(d));

            sb.Append("\n");
        }

        void DefineStructs(ShaderCode sc, StringBuilder sb)
        {
            sb.AppendFormat("\nstruct VS_IN\n{{\n");
            foreach (var i in sc.VSInput)
            {
                sb.AppendFormat("\t{0} {1} : {2}{3};\n", i.Format.ToString().ToLower(), i.Name, i.Semantic, i.SemanticIndex);
            }
            sb.AppendFormat("}};\n\n");

            //---

            sb.AppendFormat("struct VS_TO_PS\n{{\n");
            int id = 0;
            foreach (var i in sc.VSToPS)
            {
                sb.AppendFormat("\t{0} {1} : TEXCOORD{2};\n", i.Format.ToString().ToLower(), i.Name, id++);
            }
            sb.AppendFormat("\t{0} {1} : SV_POSITION;\n", sc.OutputPosition.Format.ToString().ToLower(), sc.OutputPosition.Name);
            sb.AppendFormat("}};\n\n");

            //---

            sb.AppendFormat("struct PS_OUT\n{{\n");

            foreach (var i in sc.PSOutput)
            {
                sb.AppendFormat("\t{0} {1} : SV_Target{2};\n", i.Format.ToString().ToLower(), i.Name, i.SemanticIndex);
            }

            sb.AppendFormat("}};\n\n");
        }

        void DefineVertexShader(ShaderCode sc, StringBuilder sb)
        {
            sb.Append("\nVS_TO_PS VS( VS_IN input )\n{\n");

            //variables from app
            foreach(var v in sc.VSInput)
                sb.AppendFormat("\t{0} {1} = input.{1};\n", FormatToString(v.Format), v.Name);

            sb.Append("\n");

            //instructions
            foreach (var i in sc.VSInstructions)
                sb.Append("\t" + Translate(i));

            sb.Append("\n");

            //send to ps
            sb.Append("\tVS_TO_PS output = (VS_TO_PS)0;\n");
            foreach(var v in sc.VSToPS)
                sb.AppendFormat("\toutput.{0} = {0};\n", v.Name);
            sb.AppendFormat("\toutput.{0} = {0};\n", sc.OutputPosition.Name);
            sb.Append("\treturn output;\n}\n\n");
        }

        void DefinePixelShader(ShaderCode sc, StringBuilder sb)
        {
            sb.Append("\nPS_OUT PS( VS_TO_PS input )\n{\n");

            //variables from vs
            foreach (var v in sc.VSToPS)
                sb.AppendFormat("\t{0} {1} = input.{1};\n", FormatToString(v.Format), v.Name);

            sb.Append('\n');

            //define output variables
            foreach (var o in sc.PSOutput)
            {
                if (!sc.VSToPS.Contains(o))
                    sb.AppendFormat("\t{0} {1};\n", FormatToString(o.Format), o.Name);
            }

            sb.Append('\n');

            //instructions
            foreach (var i in sc.PSInstructions)
            {
                sb.Append("\t" + Translate(i));
            }

            sb.Append('\n');

            //output from ps
            sb.Append("\tPS_OUT output = (PS_OUT)0;\n");
            foreach (var o in sc.PSOutput)
                sb.AppendFormat("\toutput.{0} = {0};\n", o.Name);
            sb.Append("\treturn output;\n}\n\n");
        }

        void DefineTechnique(ShaderCode sc, StringBuilder sb)
        {
            sb.Append(
"\ntechnique10 Render\n" +
"{\n" +
"	pass P0\n" +
"	{\n" +
"		SetGeometryShader( 0 );\n" +
"		SetVertexShader( CompileShader( vs_4_0, VS() ) );\n" +
"		SetPixelShader( CompileShader( ps_4_0, PS() ) );\n" +
"	}\n" +
"}\n"
    );
        }

    }
}
