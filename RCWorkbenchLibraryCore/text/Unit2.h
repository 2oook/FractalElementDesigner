//---------------------------------------------------------------------------

#ifndef Unit2H
#define Unit2H
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
//---------------------------------------------------------------------------
class TForm2 : public TForm
{
__published:	// IDE-managed Components
    TLabel *Label1;
    TLabel *Label2;
    TLabel *Label3;
    TEdit *Edit1;
    TEdit *Edit2;
    TEdit *Edit3;
    TGroupBox *GroupBox1;
    TButton *Button1;
    TButton *Button2;
    TRadioButton *RadioButton3;
    TLabel *Label4;
    TEdit *Edit4;
    TEdit *Edit5;
    TEdit *Edit6;
    TLabel *Label5;
    TLabel *Label6;
    TEdit *Edit7;
    TLabel *Label7;
    TRadioButton *RadioButton4;
    TRadioButton *RadioButton2;
    TRadioButton *RadioButton1;
    TRadioButton *RadioButton5;
        TEdit *Edit8;
        TLabel *Label8;
        TRadioButton *RadioButton6;
        TRadioButton *RadioButton7;
        TRadioButton *RadioButton8;
        TLabel *Label9;
        TLabel *Label10;
        TEdit *Edit9;
        TEdit *Edit10;
    void __fastcall Button2Click(TObject *Sender);
    void __fastcall StructureTypeRadioButtonClick(TObject *Sender);
    void __fastcall OnEditKeyPress(TObject *Sender, char &Key);
    void __fastcall FormActivate(TObject *Sender);
private:	// User declarations
public:		// User declarations
    __fastcall TForm2(TComponent* Owner);
};
//---------------------------------------------------------------------------
extern PACKAGE TForm2 *Form2;
//---------------------------------------------------------------------------
#endif
 