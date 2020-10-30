//---------------------------------------------------------------------------

#ifndef Unit8H
#define Unit8H
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ExtCtrls.hpp>
//---------------------------------------------------------------------------
class TForm8 : public TForm
{
__published:	// IDE-managed Components
    TGroupBox *GroupBox2;
    TLabel *Label1;
    TLabel *Label2;
    TLabel *Label3;
    TEdit *Edit1;
    TEdit *Edit2;
    TEdit *Edit3;
    TRadioGroup *RadioGroup1;
    TRadioButton *RadioButton1;
    TRadioButton *RadioButton2;
    TButton *Button1;
    TButton *Button2;
    TLabel *Label4;
    TLabel *Label5;
    TEdit *Edit4;
    TEdit *Edit5;
  TLabel *Label6;
  TLabel *Label7;
  TEdit *Edit6;
  TEdit *Edit7;
    void __fastcall Button1Click(TObject *Sender);
    void __fastcall OnEditKeyPress(TObject *Sender, char &Key);
private:	// User declarations
    bool __fastcall ValidateForm();
public:		// User declarations
    __fastcall TForm8(TComponent* Owner);

};
//---------------------------------------------------------------------------
extern PACKAGE TForm8 *Form8;
//---------------------------------------------------------------------------
#endif
