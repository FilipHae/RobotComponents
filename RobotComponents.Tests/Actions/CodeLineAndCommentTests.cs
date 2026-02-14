// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components
// Project: https://github.com/RobotComponents/RobotComponents
//
// For license details, see the LICENSE file in the project root.

// Xunit Libs
using Xunit;
// Robot Components Libs
using RobotComponents.ABB.Actions.Dynamic;
using RobotComponents.ABB.Enumerations;

namespace RobotComponents.Tests.Actions
{
    public class CodeLineTests
    {
        [Fact]
        public void InstructionType_ToRAPIDInstruction_ReturnsCode()
        {
            CodeLine cl = new CodeLine("MoveL p1;");

            Assert.Equal("MoveL p1;", cl.ToRAPIDInstruction(null));
        }

        [Fact]
        public void InstructionType_ToRAPIDDeclaration_ReturnsEmpty()
        {
            CodeLine cl = new CodeLine("MoveL p1;");

            Assert.Equal("", cl.ToRAPIDDeclaration(null));
        }

        [Fact]
        public void DeclarationType_ToRAPIDDeclaration_ReturnsCode()
        {
            CodeLine cl = new CodeLine("VAR num x;", CodeType.Declaration);

            Assert.Equal("VAR num x;", cl.ToRAPIDDeclaration(null));
        }

        [Fact]
        public void DeclarationType_ToRAPIDInstruction_ReturnsEmpty()
        {
            CodeLine cl = new CodeLine("VAR num x;", CodeType.Declaration);

            Assert.Equal("", cl.ToRAPIDInstruction(null));
        }

        [Fact]
        public void DefaultType_IsInstruction()
        {
            CodeLine cl = new CodeLine("code");

            Assert.Equal(CodeType.Instruction, cl.Type);
        }

        [Fact]
        public void IsValid_NonEmpty_ReturnsTrue()
        {
            CodeLine cl = new CodeLine("abc");

            Assert.True(cl.IsValid);
        }

        [Fact]
        public void IsValid_Empty_ReturnsFalse()
        {
            CodeLine cl = new CodeLine("");

            Assert.False(cl.IsValid);
        }

        [Fact]
        public void IsValid_Null_ReturnsFalse()
        {
            CodeLine cl = new CodeLine();

            Assert.False(cl.IsValid);
        }
    }

    public class CommentTests
    {
        [Fact]
        public void InstructionType_ToRAPIDInstruction_ReturnsBangPrefix()
        {
            Comment c = new Comment("hello");

            Assert.Equal("! hello", c.ToRAPIDInstruction(null));
        }

        [Fact]
        public void InstructionType_ToRAPIDDeclaration_ReturnsEmpty()
        {
            Comment c = new Comment("hello");

            Assert.Equal("", c.ToRAPIDDeclaration(null));
        }

        [Fact]
        public void DeclarationType_ToRAPIDDeclaration_ReturnsBangPrefix()
        {
            Comment c = new Comment("hello", CodeType.Declaration);

            Assert.Equal("! hello", c.ToRAPIDDeclaration(null));
        }

        [Fact]
        public void DeclarationType_ToRAPIDInstruction_ReturnsEmpty()
        {
            Comment c = new Comment("hello", CodeType.Declaration);

            Assert.Equal("", c.ToRAPIDInstruction(null));
        }

        [Fact]
        public void DefaultType_IsInstruction()
        {
            Comment c = new Comment("text");

            Assert.Equal(CodeType.Instruction, c.Type);
        }

        [Fact]
        public void IsValid_NonEmpty_ReturnsTrue()
        {
            Comment c = new Comment("text");

            Assert.True(c.IsValid);
        }

        [Fact]
        public void IsValid_Empty_ReturnsFalse()
        {
            Comment c = new Comment("");

            Assert.False(c.IsValid);
        }

        [Fact]
        public void IsValid_Null_ReturnsFalse()
        {
            Comment c = new Comment();

            Assert.False(c.IsValid);
        }

        [Fact]
        public void NewlineStripped_SingleArg()
        {
            Comment c = new Comment("line1\nMoveL dangerous;");

            Assert.DoesNotContain("\n", c.Com);
            Assert.Equal("! line1 MoveL dangerous;", c.ToRAPIDInstruction(null));
        }

        [Fact]
        public void NewlineStripped_TwoArg()
        {
            Comment c = new Comment("line1\r\nline2", CodeType.Declaration);

            Assert.DoesNotContain("\r", c.Com);
            Assert.DoesNotContain("\n", c.Com);
            Assert.Equal("! line1 line2", c.ToRAPIDDeclaration(null));
        }

        [Fact]
        public void SetterStripsNewlines()
        {
            Comment c = new Comment("safe");
            c.Com = "text\ninjected;";

            Assert.DoesNotContain("\n", c.Com);
            Assert.Equal("text injected;", c.Com);
        }

        [Fact]
        public void NullComment_NoException()
        {
            Comment c = new Comment();
            c.Com = null;

            Assert.Null(c.Com);
        }

        [Fact]
        public void EmptyComment_NoException()
        {
            Comment c = new Comment("");

            Assert.Equal("", c.Com);
        }

        [Fact]
        public void NoNewline_Unchanged()
        {
            Comment c = new Comment("normal comment text");

            Assert.Equal("normal comment text", c.Com);
            Assert.Equal("! normal comment text", c.ToRAPIDInstruction(null));
        }

        [Fact]
        public void CarriageReturn_Stripped()
        {
            Comment c = new Comment("line1\rline2");

            Assert.DoesNotContain("\r", c.Com);
            Assert.Equal("line1 line2", c.Com);
        }
    }
}
