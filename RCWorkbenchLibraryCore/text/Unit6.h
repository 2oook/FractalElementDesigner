//---------------------------------------------------------------------------

#ifndef Unit6H
#define Unit6H
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ExtCtrls.hpp>
//---------------------------------------------------------------------------
class TForm6 : public TForm
{
__published:	// IDE-managed Components
    TButton *Button1;
    TComboBox *ComboBox1;
    TImage *Image1;
    TLabel *Label2;
    TEdit *Edit1;
    TLabel *Label3;
    TEdit *Edit2;
    TLabel *Label4;
    TLabel *Label5;
    TButton *Button2;
    TComboBox *ComboBox2;
    TLabel *Label1;
        TEdit *Edit3;
    void __fastcall Button1Click(TObject *Sender);
    void __fastcall ComboBox1Change(TObject *Sender);
    void __fastcall FormActivate(TObject *Sender);
    void __fastcall OnEditKeyPress(TObject *Sender, char &Key);
private:	// User declarations
public:		// User declarations
    __fastcall TForm6(TComponent* Owner);
};

void UpdateControls(TComboBox *CB, TLabel *LCharacteristic, TComboBox *CBCharacteristic, TImage *ImageSch, TLabel *LK, TEdit *EK, TLabel *LL, TEdit *EL, TLabel *LKP, TEdit *EFormula);
//---------------------------------------------------------------------------
extern PACKAGE TForm6 *Form6;
//---------------------------------------------------------------------------
#endif
 