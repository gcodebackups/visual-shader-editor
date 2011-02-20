﻿/*
Copyright (c) 2011, Pawel Szczurek
All rights reserved.


Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:


Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or
other materials provided with the distribution.

Neither the name of the <ORGANIZATION> nor the names of its contributors may be used to endorse or promote products derived from this software without
specific prior written permission.


THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE
USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Text;
using Core.Basic;
using Core.Main;
using Core.CodeGeneration;
using Core.CodeGeneration.Code;

namespace Core.Blocks.Math.Other
{
    [Block(Name = "Lerp", Path = "Math/Other")]
    public class Lerp : BaseBlock
    {
        public Lerp(BlockManager owner)
            : base(owner, null)
        {
            AddInput(new BlockInput(this, "A"));
            AddInput(new BlockInput(this, "B"));
            AddInput(new BlockInput(this, "Alpha"));
            AddOutput(new BlockOutput(this, Format.FLOAT4, "Output"));
            RaiseDataChanged();
        }
        protected override void DataChanged()
        {
            if (Inputs.Count == 3 && Outputs.Count == 1)
            {
                Outputs[0].Format = InstructionHelper.FindBigestInput(Inputs[0], Inputs[1]);
            }
            base.DataChanged();
        }

        protected internal override void GenerateCode(ShaderCodeGenerator sc)
        {
            sc.AddInstruction(new CreateVariableInstruction(
                new BinaryExpression(BinaryExpression.Operators.Assign,
                    new VariableExpression(Outputs[0].Variable),
                    new CallExpression(CallExpression.Function.Lerp,
                        InstructionHelper.ConvertInputTo(Outputs[0].Format, Inputs[0]),
                        InstructionHelper.ConvertInputTo(Outputs[0].Format, Inputs[1]),
                        InstructionHelper.ConvertInputTo(Format.FLOAT, Inputs[2])))));
        }
    }
}
