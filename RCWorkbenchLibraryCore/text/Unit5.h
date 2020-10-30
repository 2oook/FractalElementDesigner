//---------------------------------------------------------------------------

#ifndef Unit5H
#define Unit5H
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
//---------------------------------------------------------------------------
class TForm5 : public TForm
{
__published:	// IDE-managed Components
    TGroupBox *GroupBox1;
    TLabel *Label1;
    TCheckBox *CheckBox1;
    TLabel *Label2;
    TLabel *Label3;
    TLabel *Label4;
    TLabel *Label5;
    TEdit *Edit1;
    TEdit *Edit2;
    TEdit *Edit3;
    TEdit *Edit4;
    TEdit *Edit5;
    TGroupBox *GroupBox2;
    TLabel *Label6;
    TLabel *Label7;
    TLabel *Label8;
    TLabel *Label9;
    TCheckBox *CheckBox2;
    TEdit *Edit6;
    TEdit *Edit7;
    TEdit *Edit8;
    TEdit *Edit9;
    TButton *Button1;
    TButton *Button2;
    TGroupBox *GroupBox3;
    TLabel *Label13;
    TEdit *Edit13;
    TCheckBox *CheckBox3;
    TGroupBox *GroupBox4;
    TLabel *Label10;
    TLabel *Label11;
    TLabel *Label12;
    TLabel *Label14;
    TEdit *Edit10;
    TEdit *Edit11;
    TEdit *Edit12;
    TEdit *Edit14;
    TCheckBox *CheckBox4;
  TCheckBox *CheckBox5;
  TCheckBox *CheckBox6;
  TGroupBox *GroupBox5;
  TLabel *Label17;
  TEdit *Edit17;
  TLabel *Label15;
  TEdit *Edit15;
  TLabel *Label16;
  TEdit *Edit16;
    void __fastcall CheckBox1Click(TObject *Sender);
    void __fastcall CheckBox2Click(TObject *Sender);
    void __fastcall OnEditKeyPress(TObject *Sender, char &Key);
    void __fastcall CheckBox3Click(TObject *Sender);
    void __fastcall CheckBox4Click(TObject *Sender);
  void __fastcall CheckBox5Click(TObject *Sender);
private:	// User declarations
public:		// User declarations
    __fastcall TForm5(TComponent* Owner);
};
//---------------------------------------------------------------------------
extern PACKAGE TForm5 *Form5;
//---------------------------------------------------------------------------
#endif
