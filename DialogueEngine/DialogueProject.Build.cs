using UnrealBuildTool;

public class DialogueProject : ModuleRules
{
	public DialogueProject(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;
	
		PublicDependencyModuleNames.AddRange(new string[] { "Core", "CoreUObject", "Engine", "InputCore", "Slate", "SlateCore" });

		PublicDependencyModuleNames.AddRange(new string[] { "DialogueProject" });

		PrivateDependencyModuleNames.AddRange(new string[] {  });
	}
}
