// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components (Modified)
// Original project: https://github.com/RobotComponents/RobotComponents
// Modified project: https://github.com/jpdrude/RobotComponents
//
// For license details, see the LICENSE file in the project root.

// Xunit Libs
using Xunit;
// Robot Components Libs
using RobotComponents.ABB.Controllers;

namespace RobotComponents.Tests.Controllers
{
    public class RunProgramSafetyTests
    {
        [Fact]
        public void IsVirtual_EmptyController_ReturnsFalse()
        {
            Controller controller = new Controller();

            Assert.False(controller.IsVirtual);
        }

        [Fact]
        public void RunProgram_EmptyController_ReturnsFalse()
        {
            Controller controller = new Controller();

            bool result = controller.RunProgram(out string status);

            Assert.False(result);
            Assert.Contains("empty", status, System.StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void IsExecutionPermitted_PhysicalNotArmed_ReturnsFalse()
        {
            Assert.False(Controller.IsExecutionPermitted(armed: false, isPhysical: true));
        }

        [Fact]
        public void IsExecutionPermitted_PhysicalArmed_ReturnsTrue()
        {
            Assert.True(Controller.IsExecutionPermitted(armed: true, isPhysical: true));
        }

        [Fact]
        public void IsExecutionPermitted_VirtualNotArmed_ReturnsTrue()
        {
            Assert.True(Controller.IsExecutionPermitted(armed: false, isPhysical: false));
        }

        [Fact]
        public void IsExecutionPermitted_VirtualArmed_ReturnsTrue()
        {
            Assert.True(Controller.IsExecutionPermitted(armed: true, isPhysical: false));
        }
    }
}
