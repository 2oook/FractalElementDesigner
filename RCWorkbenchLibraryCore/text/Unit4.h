//---------------------------------------------------------------------------

#ifndef Unit4H
#define Unit4H
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <Mask.hpp>
//---------------------------------------------------------------------------
class TForm4 : public TForm
{
__published:	// IDE-managed Components
    TGroupBox *GroupBox1;
    TLabel *Label1;
    TLabel *Label2;
    TLabel *Label3;
    TLabel *Label4;
    TLabel *Label5;
    TLabel *Label6;
    TMaskEdit *MaskEdit1;
    TMaskEdit *MaskEdit2;
    TMaskEdit *MaskEdit3;
    TMaskEdit *MaskEdit4;
    TMaskEdit *MaskEdit5;
    TMaskEdit *MaskEdit6;
    TGroupBox *GroupBox2;
    TGroupBox *GroupBox3;
    TLabel *Label7;
    TLabel *Label8;
    TLabel *Label9;
    TLabel *Label10;
    TMaskEdit *MaskEdit7;
    TMaskEdit *MaskEdit8;
    TMaskEdit *MaskEdit9;
    TMaskEdit *MaskEdit10;
    TMaskEdit *MaskEdit11;
    TMaskEdit *MaskEdit12;
    TButton *Button1;
    TButton *Button2;
    TGroupBox *GroupBox5;
    TLabel *Label11;
    TLabel *Label12;
    TLabel *Label13;
    TLabel *Label14;
    TMaskEdit *MaskEdit13;
    TMaskEdit *MaskEdit14;
    void __fastcall Button1Click(TObject *Sender);
private:	// User declarations
public:		// User declarations
    __fastcall TForm4(TComponent* Owner);
};
//---------------------------------------------------------------------------
extern PACKAGE TForm4 *Form4;
//---------------------------------------------------------------------------
#endif
