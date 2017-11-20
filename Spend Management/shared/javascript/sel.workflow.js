/// <summary>
/// "Phase One" Greenlight Workflows
/// </summary>     
(function()
{
    var scriptName = "Workflow";
    function execute()
    {
        SEL.registerNamespace("SEL.Workflow");
        SEL.Workflow =
        {
            /// <summary>
            /// Workflow Step Type Enumerator
            /// </summary>   
            StepTypeEnum:
            {
                None: { ToInt: (function() { return 0; }), ToString: (function() { return "Please select a step type"; }) },
                Approval: { ToInt: (function() { return 1; }), ToString: (function() { return "Approval"; }) },
                ChangeValue: { ToInt: (function() { return 2; }), ToString: (function() { return "Change Value"; }) },
                CheckCondition: { ToInt: (function() { return 3; }), ToString: (function() { return "Check Condition"; }) },
                Decision: { ToInt: (function() { return 4; }), ToString: (function() { return "Decision"; }) },
                MoveToStep: { ToInt: (function() { return 5; }), ToString: (function() { return "Move To Step"; }) },
                RunSubWorkflow: { ToInt: (function() { return 6; }), ToString: (function() { return "Run Sub Workflow"; }) },
                SendEmail: { ToInt: (function() { return 7; }), ToString: (function() { return "Send Email"; }) },
                ElseCondition: { ToInt: (function() { return 8; }), ToString: (function() { return "Else Condition"; }) },
                FinishWorkflow: { ToInt: (function() { return 9; }), ToString: (function() { return "Finish Workflow"; }) },
                ElseOtherwise: { ToInt: (function() { return 10; }), ToString: (function() { return "Else Otherwise"; }) },
                CreateDynamicValue: { ToInt: (function() { return 11; }), ToString: (function() { return "Create Dynamic Value"; }) },
                DecisionFalse: { ToInt: (function() { return 12; }), ToString: (function() { return "Decision False"; }) },
                ShowMessage: { ToInt: (function() { return 13; }), ToString: (function() { return "Show Message"; }) },
                ChangeForm: { ToInt: (function() { return 14; }), ToString: (function() { return "Change Form"; }) }
            },
            /// <summary>
            /// Workflow Step Type Enumerator
            /// </summary>   
            ConvertToStepTypeEnum: function(val)
            {
                switch (val)
                {
                    case 0:
                    case "0":
                    case "None":
                    case "Please select a step type":
                        return this.StepTypeEnum.None;
                    case 1:
                    case "1":
                    case "Approval":
                        return this.StepTypeEnum.Approval;
                    case 2:
                    case "2":
                    case "Change Value":
                        return this.StepTypeEnum.ChangeValue;
                    case 3:
                    case "3":
                    case "Check Condition":
                        return this.StepTypeEnum.CheckCondition;
                    case 4:
                    case "4":
                    case "Decision":
                        return this.StepTypeEnum.Decision;
                    case 5:
                    case "5":
                    case "Move To Step":
                        return this.StepTypeEnum.MoveToStep;
                    case 6:
                    case "6":
                    case "Run Sub Workflow":
                        return this.StepTypeEnum.RunSubWorkflow;
                    case 7:
                    case "7":
                    case "Send Email":
                        return this.StepTypeEnum.SendEmail;
                    case 8:
                    case "8":
                    case "Else Condition":
                        return this.StepTypeEnum.ElseCondition;
                    case 9:
                    case "9":
                    case "Finish Workflow":
                        return this.StepTypeEnum.FinishWorkflow;
                    case 10:
                    case "10":
                    case "Else Otherwise":
                        return this.StepTypeEnum.ElseOtherwise;
                    case 11:
                    case "11":
                    case "Create Dynamic Value":
                        return this.StepTypeEnum.CreateDynamicValue;
                    case 12:
                    case "12":
                    case "Decision False":
                        return this.StepTypeEnum.DecisionFalse;
                    case 13:
                    case "13":
                    case "Show Message":
                        return this.StepTypeEnum.ShowMessage;
                    case 14:
                    case "14":
                    case "Change Form":
                        return this.StepTypeEnum.ChangeForm;
                    default:
                        return this.StepTypeEnum.None;
                }
            },
            /// <summary>
            /// Workflow Steps Collection
            /// </summary>
            steps: [],
            /// <summary>
            /// Workflow Step Object
            /// </summary>   
            Step: function()
            {
                this.id = null;
                this.divID = null;
                this.action = SEL.Workflow.StepTypeEnum.None;
                this.description = null;
                this.question = null;
                this.trueAnswer = null;
                this.falseAnswer = null;
                this.conditions = [];
                this.approverType = null;
                this.actionID = null;
                this.oneClickSignoff = null;
                this.showDeclaration = null;
                this.parentStepID = null;
                this.relatedStepID = null;
                this.approvalEmailTemplateID = null;
                this.message = null;
                this.formID = null;
            },
            /// <summary>
            /// Populate Workflow Steps Collection from page array
            /// </summary>
            GetSteps: function()
            {
                var i;
                var tmpStep;
                for (i = 0; i < currentSteps.length; i++)
                {
                    if (currentSteps[i] !== null)
                    {
                        tmpStep = new this.Step();

                        tmpStep.id = parseInt(currentSteps[i]["divID"].substr(5), 0);
                        tmpStep.divID = currentSteps[i]["divID"];
                        tmpStep.parentStepID = currentSteps[i]["parentStepID"];
                        tmpStep.relatedStepID = currentSteps[i]["relatedStepID"];
                        tmpStep.action = this.ConvertToStepTypeEnum(currentSteps[i]["action"]);
                        tmpStep.actionID = currentSteps[i]["actionID"];
                        tmpStep.approvalEmailTemplateID = currentSteps[i]["approvalEmailTemplateID"];
                        tmpStep.approverType = currentSteps[i]["approverType"];
                        tmpStep.conditions = currentSteps[i]["conditions"];
                        tmpStep.description = currentSteps[i]["description"];
                        tmpStep.falseAnswer = currentSteps[i]["falseAnswer"];
                        tmpStep.formID = currentSteps[i]["formID"];
                        tmpStep.message = currentSteps[i]["message"];
                        tmpStep.oneClickSignoff = currentSteps[i]["oneclicksignoff"];
                        tmpStep.question = currentSteps[i]["question"];
                        tmpStep.showDeclaration = currentSteps[i]["showdeclaration"];
                        tmpStep.trueAnswer = currentSteps[i]["trueAnswer"];

                        this.steps.push( tmpStep );
                    }
                }
                return this.steps;
            },
            /// <summary>
            /// Called when a workflow is fetched by web service
            /// </summary>     
            onComplete: function()
            {
                return;
            },
            /// <summary>
            /// If an error occurs
            /// </summary>                
            onError: function()
            {
                showMasterPopup("Error in workflow");
                return;
            }
        };
    }

    if (window.Sys && Sys.loader)
    {
        Sys.loader.registerScript(scriptName, null, execute);
    }
    else
    {
        execute();
    }
}
)();