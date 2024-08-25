#include "TestFunctionLibrary.h"
#include "Blueprint/UserWidget.h"
#include "MyProject2/MyUserWidget.h"
#include "Kismet/GameplayStatics.h"
#include "Engine/Engine.h"

void UTestFunctionLibrary::CreateNPCDialogue(FString dialogue, float char_delay, TSubclassOf<UMyUserWidget> widgettemplate, UMyUserWidget* npcdialogueclass, APlayerController* playercontroller)
{
	UWorld* world = playercontroller->GetWorld();
	npcdialogueclass = CreateWidget<UMyUserWidget>(world, widgettemplate);

	npcdialogueclass->message = dialogue;
	npcdialogueclass->char_delay = char_delay;

	npcdialogueclass->AddToViewport();
	npcdialogueclass->SetIsFocusable(true);
	npcdialogueclass->SetKeyboardFocus();
	npcdialogueclass->SetFocus();

	npcdialogueclass->DisplayDialogue();

	return;
}