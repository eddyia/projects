#include "MyUserWidget.h"
#include "Components/TextBlock.h"
#include "TimerManager.h"
#include "Sound/SoundBase.h"
#include "Components/Image.h"
#include "Engine/Texture2D.h"
#include <Slate.h>
#include "Components/AudioComponent.h"

int CountNumberDialogues(std::string input)
{
	int num_dialogue_boxes = 0;

	for (int i = 0; i < input.length(); i++)
	{
		if (input[i] == '~')
		{
			num_dialogue_boxes++;
		}
	}

	return num_dialogue_boxes;
}

//Constructor for UMyUserWidget class
void UMyUserWidget::NativeConstruct()
{
	Super::NativeConstruct();

	bIsFocusable = true;
}

//Deconstructor for UMyUserWidget class
void UMyUserWidget::NativeDestruct()
{
	Super::NativeDestruct();

	GetWorld()->GetTimerManager().ClearTimer(display_char_timer);
	GetWorld()->GetTimerManager().ClearTimer(change_properties_timer);
}

void UMyUserWidget::DisplayDialogue()
{
	FInputModeUIOnly c;
	GetWorld()->GetFirstPlayerController()->SetInputMode(c);

	std::string raw_dialogue = TCHAR_TO_UTF8(*message);

	num_dialogue_boxes = CountNumberDialogues(raw_dialogue);
	dialogues.reserve(num_dialogue_boxes);

	/// Populate Dialogue Vector & Character Array 
	int dialogue_start_pos = 1;
	int dialogue_end_pos;
	int dialogue_substring_len;

	int character_start_pos = 0;
	int character_end_pos = 0;
	char character_char;

	for (int i = 0; i < num_dialogue_boxes; i++)
	{
		character_start_pos = raw_dialogue.find('~', character_start_pos) + 1;
		character_char = raw_dialogue[character_start_pos];

		UE_LOG(LogTemp, Log, TEXT("Character char was: %c"), character_char);

		character_start_pos += 1;

		dialogue_end_pos = raw_dialogue.find('~', dialogue_start_pos + 1);
		dialogue_substring_len = (dialogue_end_pos - dialogue_start_pos);

		dialogues.push_back(raw_dialogue.substr(dialogue_start_pos + 2, dialogue_substring_len - 2));

		dialogue_start_pos = dialogue_end_pos + 1;

		switch (character_char) {
		case 'D':
			current_character[i] = Durn;
			//UE_LOG(LogTemp, Log, TEXT("Current character %i is Durn"), i);
			break;

		case 'V':
			current_character[i] = Vayla;
			//UE_LOG(LogTemp, Log, TEXT("Current character %i is Vayla"), i);
			break;
		};

		character_start_pos += 1;
	}
	///

	FString file_path;
	FSlateBrush brush;
	
	switch (current_character[0]) {
		case Durn:
			file_path = TEXT("/Script/Engine.Texture2D'/Game/Durn.Durn'");
			npc_texture = Cast<UTexture2D>(StaticLoadObject(UTexture2D::StaticClass(), nullptr, *file_path));
			brush.SetResourceObject(npc_texture);
			npc_face->SetBrush(brush);

			file_path = TEXT("/Script/Engine.SoundWave'/Game/durndialogue.durndialogue'");
			babble_sound_effect = Cast<USoundBase>(StaticLoadObject(USoundBase::StaticClass(), nullptr, *file_path));
			break;

		case Vayla:
			file_path = TEXT("/Script/Engine.Texture2D'/Game/Vayla.Vayla'");
			npc_texture = Cast<UTexture2D>(StaticLoadObject(UTexture2D::StaticClass(), nullptr, *file_path));
			brush.SetResourceObject(npc_texture);
			npc_face->SetBrush(brush);

			file_path = TEXT("/Script/Engine.SoundWave'/Game/vayladialogue.vayladialogue'");
			babble_sound_effect = Cast<USoundBase>(StaticLoadObject(USoundBase::StaticClass(), nullptr, *file_path));

			break;
	};

	current_character_index += 1;

	GetWorld()->GetTimerManager().SetTimer(display_char_timer, this, &UMyUserWidget::DisplayChars, char_delay, true, 0.0f);

	return;
}

void UMyUserWidget::DisplayChars()
{
	//If the current length of the substring is not equal to the length of the actual message, continue incrementing integer length until they are equal (defined in MyUserWidget.h)
	if (substring_length != dialogues[current_dialogue].length() + 1)
	{
		//Set the current text message to be a substring from index 0 to length
		FString parsed_message = FString((UTF8_TO_TCHAR(dialogues[current_dialogue].c_str())));

		//UE_LOG(LogTemp, Log, TEXT("Parsed message is: %s with a length of %i"), *parsed_message, dialogues[num_calls].length() + 1);

		npc_dialogue->SetText((FText::FromString(parsed_message.Mid(0, substring_length))));
		substring_length++;
		PlaySound(babble_sound_effect);
	}
	//The message is fully displayed, therefore "Press space to continue" should pop up and the timer should be cleared.
	else
	{
		space_message->SetText(FText::FromString("Press space to continue"));
		GetWorld()->GetTimerManager().ClearTimer(display_char_timer);
		message_displayed = true;
		current_dialogue++;
	}
}

void UMyUserWidget::ChangeProperties()
{
	//Change image and babble sound depending on the character based on when the user presses the space bar
	FString file_path;
	FSlateBrush brush;

	switch (current_character[current_character_index])
	{
		case Durn:
			file_path = TEXT("/Script/Engine.Texture2D'/Game/Durn.Durn'");
			npc_texture = Cast<UTexture2D>(StaticLoadObject(UTexture2D::StaticClass(), nullptr, *file_path));
			brush.SetResourceObject(npc_texture);
			npc_face->SetBrush(brush);

			file_path = TEXT("/Script/Engine.SoundWave'/Game/durndialogue.durndialogue'");
			babble_sound_effect = Cast<USoundBase>(StaticLoadObject(USoundBase::StaticClass(), nullptr, *file_path));
			break;

		case Vayla:
			file_path = TEXT("/Script/Engine.Texture2D'/Game/Vayla.Vayla'");
			npc_texture = Cast<UTexture2D>(StaticLoadObject(UTexture2D::StaticClass(), nullptr, *file_path));
			brush.SetResourceObject(npc_texture);
			npc_face->SetBrush(brush);

			file_path = TEXT("/Script/Engine.SoundWave'/Game/vayladialogue.vayladialogue'");
			babble_sound_effect = Cast<USoundBase>(StaticLoadObject(USoundBase::StaticClass(), nullptr, *file_path));
			break;
	};

	current_character_index += 1;

	SetVisibility(ESlateVisibility::Visible);
	GetWorld()->GetTimerManager().ClearTimer(change_properties_timer);
}

FReply UMyUserWidget::NativeOnKeyDown(const FGeometry& InGeometry, const FKeyEvent& InKeyEvent)
{
	if (message_displayed && InKeyEvent.GetKey() == EKeys::SpaceBar)
	{
		//Checks if we are at the end of the dialogue
		if (current_character_index == dialogues.size())
		{
			FInputModeGameOnly c;
			GetWorld()->GetFirstPlayerController()->SetInputMode(c);

			//Remove widget from screen and call the deconstructor
			RemoveFromParent();
		}

		//Remove widget from player screen
		SetVisibility(ESlateVisibility::Hidden);
		message_displayed = false;
		substring_length = 0;

		space_message->SetText(FText::FromString(""));
		npc_dialogue->SetText(FText::FromString(""));

		GetWorld()->GetTimerManager().SetTimer(change_properties_timer, this, &UMyUserWidget::ChangeProperties, 0.1f, true, 0.95f);
		GetWorld()->GetTimerManager().SetTimer(display_char_timer, this, &UMyUserWidget::DisplayChars, char_delay, true, 1.0f);

		return FReply::Handled();
	}

	return Super::NativeOnKeyDown(InGeometry, InKeyEvent);
}

