//---------------------------------------------------------------------------

#ifndef Unit7H
#define Unit7H
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ExtCtrls.hpp>
//---------------------------------------------------------------------------
class TForm7 : public TForm
{
__published:	// IDE-managed Components
    TComboBox *ComboBox2;
    TGroupBox *GroupBox2;
    TLabel *Label1;
    TLabel *Label2;
    TLabel *Label3;
    TEdit *Edit1;
    TEdit *Edit2;
    TEdit *Edit3;
    TButton *Button1;
    TButton *Button2;
    TRadioGroup *RadioGroup1;
    TRadioButton *RadioButton1;
    TRadioButton *RadioButton2;
    TImage *Image1;
    TPanel *Panel1;
    TLabel *Label4;
    TLabel *Label5;
    TLabel *Label6;
    TEdit *Edit4;
    TEdit *Edit5;
    TEdit *Edit6;
    TEdit *Edit7;
    TEdit *Edit8;
    TLabel *Label7;
    TLabel *Label8;
    TPanel *Panel2;
    TLabel *Label9;
    TLabel *Label10;
    TLabel *Label11;
    TLabel *Label12;
    TLabel *Label13;
    TEdit *Edit9;
    TEdit *Edit10;
    TEdit *Edit11;
    TEdit *Edit12;
    TEdit *Edit13;
    TPanel *Panel3;
    TLabel *Label14;
    TLabel *Label15;
    TLabel *Label16;
    TLabel *Label17;
    TLabel *Label18;
    TEdit *Edit14;
    TEdit *Edit15;
    TEdit *Edit16;
    TEdit *Edit17;
    TEdit *Edit18;
    TLabel *Label19;
    TEdit *Edit19;
    TLabel *Label20;
    TEdit *Edit20;
    TLabel *Label21;
    TEdit *Edit21;
    TPanel *Panel4;
    TLabel *Label22;
    TLabel *Label23;
    TLabel *Label24;
    TLabel *Label25;
    TLabel *Label26;
    TLabel *Label27;
    TLabel *Label28;
    TLabel *Label29;
    TEdit *Edit22;
    TEdit *Edit23;
    TEdit *Edit24;
    TEdit *Edit25;
    TEdit *Edit26;
    TEdit *Edit27;
    TEdit *Edit28;
    TEdit *Edit29;
    TLabel *Label30;
    TEdit *Edit30;
    TLabel *Label31;
    void __fastcall ComboBox2Change(TObject *Sender);
    void __fastcall Button1Click(TObject *Sender);
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall OnEditKeyPress(TObject *Sender, char &Key);
    void __fastcall RadioButton1Click(TObject *Sender);
    void __fastcall RadioButton2Click(TObject *Sender);
    void __fastcall OnEditChange(TObject *Sender);
private:	// User declarations
    void __fastcall DrawGraph(int index);
    bool __fastcall ValidateForm(int index);
public:		// User declarations
    __fastcall TForm7(TComponent* Owner);
};
//---------------------------------------------------------------------------
extern PACKAGE TForm7 *Form7;
//---------------------------------------------------------------------------
#endif
