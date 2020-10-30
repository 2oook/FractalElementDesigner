
//#include <stdafx.h>
#include "proots.h"

/*************************************************************************
Поиск корней полинома методом собственных значений.

Алгоритм ищет   корни   полинома   путем   поиска   собственных   значений
его матрицы-компаньона.

Входные данные:
    C   -   массив коэффициентов полинома.
            Нумерация элементов от 0 до N.
            C[I] - коэффициент при X^I
    N   -   степень полинома. N>=1.
    
Выходные данные:
    WR  -   массив вещественных частей корней полинома.
            Нумерация элементов от 1 до N.
    WI  -   массив мнимых частей корней полинома.
            Нумерация элементов от 1 до N.
При этом комплексно-сопряженные корни идут один за другим.

Результат:
    True,  если алгоритм поиска собственных значений сошелся.
    False, если алгоритм поиска собственных значений не сошелся.
*************************************************************************/
bool eigenpolyroots(const ap::real_1d_array& c,
     int n,
     ap::real_1d_array& wr,
     ap::real_1d_array& wi)
{
    bool result;
    ap::real_2d_array a;
    int nn;
    int m;
    int l;
    int k;
    int j;
    int its;
    int i;
    int mmin;
    double z;
    double y;
    double x;
    double w;
    double v;
    double u;
    double t;
    double s;
    double r;
    double q;
    double p;
    double anorm;
    bool cycleflag;

    ap::ap_error::make_assertion(n>=1, "EigenPolyRoots: N<1!");
    
    //
    // Prepare A
    //
    a.setbounds(1, n, 1, n);
    for(i = 1; i <= n; i++)
    {
        for(j = 1; j <= n; j++)
        {
            a(i,j) = 0;
        }
    }
    for(i = 2; i <= n; i++)
    {
        a(i,i-1) = 1;
    }
    for(j = 1; j <= n; j++)
    {
        a(1,j) = -c(n-j)/c(n);
    }
    
    //
    // Prepare EVD routine
    //
    wr.setbounds(1, n);
    wi.setbounds(1, n);
    result = true;
    
    //
    // Search eigenvalues of A
    //
    anorm = fabs(a(1,1));
    for(i = 2; i <= n; i++)
    {
        for(j = i-1; j <= n; j++)
        {
            anorm = anorm+fabs(a(i,j));
        }
    }
    nn = n;
    t = 0.0;
    cycleflag = true;
    while(nn>=1)
    {
        if( cycleflag )
        {
            its = 0;
        }
        cycleflag = true;
        for(l = nn; l >= 2; l--)
        {
            s = fabs(a(l-1,l-1))+fabs(a(l,l));
            if( s==0 )
            {
                s = anorm;
            }
            if( fabs(s*100*ap::machineepsilon)>=fabs(a(l,l-1)) )
            {
                break;
            }
        }
        x = a(nn,nn);
        if( l==nn )
        {
            wr(nn) = x+t;
            wi(nn) = 0.0;
            nn = nn-1;
        }
        else
        {
            y = a(nn-1,nn-1);
            w = a(nn,nn-1)*a(nn-1,nn);
            if( l==nn-1 )
            {
                p = 0.5*(y-x);
                q = ap::sqr(p)+w;
                z = sqrt(fabs(q));
                x = x+t;
                if( q>=0 )
                {
                    if( p<0 )
                    {
                        z = p-fabs(z);
                    }
                    else
                    {
                        z = p+fabs(z);
                    }
                    wr(nn) = x+z;
                    wr(nn-1) = wr(nn);
                    if( z!=0 )
                    {
                        wr(nn) = x-w/z;
                    }
                    wi(nn) = 0.0;
                    wi(nn-1) = 0.0;
                }
                else
                {
                    wr(nn) = x+p;
                    wr(nn-1) = wr(nn);
                    wi(nn) = z;
                    wi(nn-1) = -z;
                }
                nn = nn-2;
            }
            else
            {
                if( its==30 )
                {
                    result = false;
                    return result;
                }
                if( its==10||its==20 )
                {
                    t = t+x;
                    for(i = 1; i <= nn; i++)
                    {
                        a(i,i) = a(i,i)-x;
                    }
                    s = fabs(a(nn,nn-1))+fabs(a(nn-1,nn-2));
                    x = 0.75*s;
                    y = x;
                    w = -0.4375*ap::sqr(s);
                }
                its = its+1;
                for(m = nn-2; m >= l; m--)
                {
                    z = a(m,m);
                    r = x-z;
                    s = y-z;
                    p = (r*s-w)/a(m+1,m)+a(m,m+1);
                    q = a(m+1,m+1)-z-r-s;
                    r = a(m+2,m+1);
                    s = fabs(p)+fabs(q)+fabs(r);
                    p = p/s;
                    q = q/s;
                    r = r/s;
                    if( m==l )
                    {
                        break;
                    }
                    u = fabs(a(m,m-1))*(fabs(q)+fabs(r));
                    v = fabs(p)*(fabs(a(m-1,m-1))+fabs(z)+fabs(a(m+1,m+1)));
                    if( fabs(v*100*ap::machineepsilon)>=fabs(u) )
                    {
                        break;
                    }
                }
                for(i = m+2; i <= nn; i++)
                {
                    a(i,i-2) = 0;
                    if( i!=m+2 )
                    {
                        a(i,i-3) = 0;
                    }
                }
                for(k = m; k <= nn-1; k++)
                {
                    if( k!=m )
                    {
                        p = a(k,k-1);
                        q = a(k+1,k-1);
                        r = 0;
                        if( k!=nn-1 )
                        {
                            r = a(k+2,k-1);
                        }
                        x = fabs(p)+fabs(q)+fabs(r);
                        if( x!=0 )
                        {
                            p = p/x;
                            q = q/x;
                            r = r/x;
                        }
                    }
                    if( p<0 )
                    {
                        s = -fabs(sqrt(ap::sqr(p)+ap::sqr(q)+ap::sqr(r)));
                    }
                    else
                    {
                        s = fabs(sqrt(ap::sqr(p)+ap::sqr(q)+ap::sqr(r)));
                    }
                    if( s!=0 )
                    {
                        if( k==m )
                        {
                            if( l!=m )
                            {
                                a(k,k-1) = -a(k,k-1);
                            }
                        }
                        else
                        {
                            a(k,k-1) = -s*x;
                        }
                        p = p+s;
                        x = p/s;
                        y = q/s;
                        z = r/s;
                        q = q/p;
                        r = r/p;
                        for(j = k; j <= nn; j++)
                        {
                            p = a(k,j)+q*a(k+1,j);
                            if( k!=nn-1 )
                            {
                                p = p+r*a(k+2,j);
                                a(k+2,j) = a(k+2,j)-p*z;
                            }
                            a(k+1,j) = a(k+1,j)-p*y;
                            a(k,j) = a(k,j)-p*x;
                        }
                        if( nn<k+3 )
                        {
                            mmin = nn;
                        }
                        else
                        {
                            mmin = k+3;
                        }
                        for(i = l; i <= mmin; i++)
                        {
                            p = x*a(i,k)+y*a(i,k+1);
                            if( k!=nn-1 )
                            {
                                p = p+z*a(i,k+2);
                                a(i,k+2) = a(i,k+2)-p*r;
                            }
                            a(i,k+1) = a(i,k+1)-p*q;
                            a(i,k) = a(i,k)-p;
                        }
                    }
                }
                cycleflag = false;
                continue;
            }
        }
    }
    return result;
}



