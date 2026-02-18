// SPDX-License-Identifier: GPL-3.0-or-later
// This file is part of Robot Components (Modified)
// Original project: https://github.com/RobotComponents/RobotComponents
// Modified project: https://github.com/jpdrude/RobotComponents
//
// Copyright (c) 2022-2025 Arjen Deetman
// Copyright (c) 2025 EDEK Uni Kassel
//
// Original Authors:
//   - Arjen Deetman (2022-2025)
//
// Modified by:
//   - Jan Philipp Drude (2025-2026)
//
// For license details, see the LICENSE file in the project root.

// System Libs
using System;
// Grasshopper Libs
using Grasshopper.Kernel;
// Robot Components Libs
using RobotComponents.ABB.Controllers;
using RobotComponents.ABB.Gh.Parameters.Controllers;

namespace RobotComponents.ABB.Gh.Components.ControllerUtility
{
    /// <summary>
    /// Represents the component that runs a program.
    /// </summary>
    public class RunProgramComponent : GH_RobotComponent
    {
        #region fields
        private Controller _controller;
        private string _status = "-";
        private bool _succeeded = true;
        #endregion

        /// <summary>
        /// Initializes a new instance of the RunProgramComponent class.
        /// </summary>
        public RunProgramComponent() : base("Run Program", "RP", "Controller Utility",
              "Starts and stops RAPID programs directly on a real or virtual ABB controller."
                + System.Environment.NewLine + System.Environment.NewLine +
                "This component uses the ABB PC SDK.")
        {
            this.Message = "DISARMED";
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Controller(), "Controller", "C", "Controller as Controller", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Arm", "A", "Arm the safety interlock as bool. Required before running on a physical controller. Not required for virtual controllers.", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Run", "R", "Run as bool", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Stop", "S", "Stop/Pause as bool", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Reset", "Re", "Resets the program pointer of all tasks as bool", GH_ParamAccess.item, false);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Status", "S", "Controller status.", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Check the operating system
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "This component is only supported on Windows operating systems.");
                return;
            }

            // Declare input variables
            bool arm = false;
            bool run = false;
            bool stop = false;
            bool reset = false;

            // Catch the input data
            if (!DA.GetData(0, ref _controller)) { return; }
            if (!DA.GetData(1, ref arm)) { arm = false; }
            if (!DA.GetData(2, ref run)) { run = false; }
            if (!DA.GetData(3, ref stop)) { stop = false; }
            if (!DA.GetData(4, ref reset)) { reset = false; }

            // Determine controller type and update interlock state
            bool isPhysical = !_controller.IsEmpty && !_controller.IsVirtual;
            bool armed = IsExecutionPermitted(arm, isPhysical);
            this.Message = isPhysical ? (arm ? "ARMED" : "DISARMED") : "VIRTUAL";

            if (run)
            {
                if (!armed)
                {
                    _succeeded = false;
                    _status = "Program not started: Arm the interlock before running on a physical controller.";
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, _status);
                }
                else
                {
                    _succeeded = _controller.RunProgram(out _status);

                    if (!_succeeded)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, _status);
                    }
                }
            }

            if (stop)
            {
                _succeeded = _controller.StopProgram(out _status);

                if (!_succeeded)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, _status);
                }
            }

            if (reset)
            {
                _succeeded = _controller.ResetProgramPointers(out _status);

                if (!_succeeded)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, _status);
                }
            }

            // Output
            DA.SetData(0, _status);
        }

        /// <summary>
        /// Determines whether program execution is permitted given the armed state and controller type.
        /// </summary>
        /// <param name="armed"> Whether the safety interlock has been armed via the Arm input. </param>
        /// <param name="isPhysical"> Whether the target controller is a physical (non-virtual) controller. </param>
        /// <returns>
        /// <c>true</c> if execution is permitted; <c>false</c> if the interlock blocks execution.
        /// Virtual controllers always permit execution regardless of the armed state.
        /// Physical controllers require <paramref name="armed"/> to be <c>true</c>.
        /// </returns>
        public static bool IsExecutionPermitted(bool armed, bool isPhysical)
        {
            if (!isPhysical) return true;
            return armed;
        }

        #region properties
        /// <summary>
        /// Override the component exposure (makes the tab subcategory).
        /// Can be set to hidden, primary, secondary, tertiary, quarternary, quinary, senary, septenary and obscure
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Gets whether this object is obsolete.
        /// </summary>
        public override bool Obsolete
        {
            get { return false; }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get { return Properties.Resources.RunProgram_Icon; }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("89A12C4C-EA6F-435A-A068-9E4806FFFF62"); }
        }
        #endregion
    }
}
