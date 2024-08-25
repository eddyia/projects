#pragma once

#include "CoreMinimal.h"
#include "Blueprint/UserWidget.h"
#include "Components/Image.h"
#include "Engine/Texture2D.h"
#include "MyUserWidget.generated.h"

UCLASS()
class DialogueProject_API UMyUserWidget : public UUserWidget
{
	GENERATED_BODY()

protected:
	virtual void NativeConstruct() override;
	virtual void NativeDestruct() override;
private:
	void DisplayChars();
	virtual FReply NativeOnKeyDown(const FGeometry& InGeometry, const FKeyEvent& InKeyEvent) override;
	void ChangeProperties();

	enum character {
		Durn,
		Vayla
	};

	character current_character[32];

	FTimerHandle display_char_timer;
	FTimerHandle change_properties_timer;

	int substring_length = 1;
	int current_dialogue = 0;
	bool message_displayed = false;
	int current_character_index = 0;
	
	UTexture2D* npc_texture;

	std::vector<std::string> dialogues;

public:
	void DisplayDialogue();

	UPROPERTY(EditAnywhere, meta = (BindWidget))
	TObjectPtr<class UImage> dialogue_box = nullptr;

	UPROPERTY(EditAnywhere, meta = (BindWidget))
	TObjectPtr<class UTextBlock> npc_dialogue = nullptr;

	UPROPERTY(EditAnywhere, meta = (BindWidget))
	TObjectPtr<class UTextBlock> space_message = nullptr;

	UPROPERTY(EditAnywhere, meta = (BindWidget))
	TObjectPtr<class UImage> npc_face= nullptr;

	FString message;
	USoundBase* babble_sound_effect;

	float char_delay;
	int num_dialogue_boxes;
};
