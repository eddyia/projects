#pragma once

#include "CoreMinimal.h"
#include "Kismet/BlueprintFunctionLibrary.h"
#include "TestFunctionLibrary.generated.h"

UCLASS()
class MYPROJECT2_API UTestFunctionLibrary : public UBlueprintFunctionLibrary
{
	GENERATED_BODY()

	UFUNCTION(BlueprintCallable)
	static void CreateNPCDialogue(FString dialogue, float char_delay, TSubclassOf<UMyUserWidget> widgettemplate, UMyUserWidget* npcdialogueclass, APlayerController* playercontroller);
};
