#pragma once

#include "CoreMinimal.h"
#include "Kismet/BlueprintFunctionLibrary.h"
#include "GlobalFunctionLibrary.generated.h"

UCLASS()
class DialogueProject_API UGlobalFunctionLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

	UFUNCTION(BlueprintCallable)
	static void CreateNPCDialogue(FString dialogue, float char_delay, TSubclassOf<UMyUserWidget> widgettemplate, UMyUserWidget* npcdialogueclass, APlayerController* playercontroller);
};
