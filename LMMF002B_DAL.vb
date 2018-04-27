Imports TFB.CMLS.Common.ADO
　
Namespace DAL
    ''' <summary>
    ''' 徵信報告-ff10版
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>　
    ''' '請註明每次更新程式的日期及姓名，當做重大更新時請更新版本
    '''  xx.　 YYYY/MM/DD    VER       AUTHOR       COMMENTS
    '''   1. 　2014/11/11    1.0       Fiona Lin    Create
    ''' </history>
    Public Class LMMF002B_DAL
　
#Region "共用變數"
　
#End Region
　
#Region "Public Method"
　
#Region " Query "
　
#Region " TB_CMLM_CASE_CRDRPT_MAIN "
　
        ''' <summary>
        ''' TB_CMLM_CASE_CRDRPT_MAIN
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 1.2014/11/11  Fiona Lin    Create
        ''' </history>
        Public Function GetMainData() As String
　
            Dim strSQL As New System.Text.StringBuilder
　
            strSQL.AppendLine(" SELECT                           ")
            strSQL.AppendLine("         CASE_SN                  ")
            strSQL.AppendLine("     , CUST_NM                    ")
            strSQL.AppendLine("     , CUST_ID                    ")
            strSQL.AppendLine("     , RG_ADDR                    ")
            strSQL.AppendLine("     , RG_ADDR_TEL                ")
            strSQL.AppendLine("     , BUS_ADDR                   ")
            strSQL.AppendLine("     , BUS_ADDR_TEL               ")
            strSQL.AppendLine("     , ADDR_MEMO                  ")
            strSQL.AppendLine("     , RG_PIC_NM                  ")
            strSQL.AppendLine("     , PIC_NM                     ")
            strSQL.AppendLine("         , (EST_DT_Y - 1911) AS EST_DT_Y")
            strSQL.AppendLine("     , EST_DT_M                   ")
            strSQL.AppendLine("     , EST_DT_D                   ")
            strSQL.AppendLine("     , EST_YEAR                   ")
            strSQL.AppendLine("     , EST_MONTH                  ")
            strSQL.AppendLine("     , SALE_KND_DESC              ")
            strSQL.AppendLine("     , TAX_IND_CODE               ")
            strSQL.AppendLine("         , (CASE WHEN COMP_LCS = 'Y' THEN 1 ELSE 0 END) AS COMP_LCS_Y ")
            strSQL.AppendLine("         , (CASE WHEN COMP_LCS = 'N' THEN 1 ELSE 0 END) AS COMP_LCS_N ")
            strSQL.AppendLine("         , (CASE WHEN FAC_LICS = 'Y' THEN 1 ELSE 0 END) AS FAC_LICS_Y ")
            strSQL.AppendLine("         , (CASE WHEN FAC_LICS = 'N' THEN 1 ELSE 0 END) AS FAC_LICS_N ")
            strSQL.AppendLine("         , (CASE WHEN COLL_RG = 'N' THEN 1 ELSE 0 END) AS COLL_RG_N ")
            strSQL.AppendLine("         , (CASE WHEN COLL_RG = 'B' THEN 1 ELSE 0 END) AS COLL_RG_B ")
            strSQL.AppendLine("         , (CASE WHEN COLL_RG = '3' THEN 1 ELSE 0 END) AS COLL_RG_3 ")
            strSQL.AppendLine("         , (CASE WHEN COLL_RG = 'O' THEN 1 ELSE 0 END) AS COLL_RG_O ")
            strSQL.AppendLine("     , PIC_CHG_TIMES              ")
            strSQL.AppendLine("     , VST_OBJ_REL                ")
            strSQL.AppendLine("         ,'0' AS VST_OBJ_REL_1 ")
            strSQL.AppendLine("         ,'0' AS VST_OBJ_REL_2 ")
            strSQL.AppendLine("         ,'0' AS VST_OBJ_REL_4 ")
            strSQL.AppendLine("         ,'0' AS VST_OBJ_REL_8 ")
            strSQL.AppendLine("     , VST_OBJ_TEL                ")
            strSQL.AppendLine("     , VST_CO_SUGG                ")
            strSQL.AppendLine("     , VST_DT                     ")
            strSQL.AppendLine("     , VST_OBJ                    ")
            strSQL.AppendLine("     , SCORE_LEVEL                ")
            strSQL.AppendLine("     , LOAN_DESC                  ")
            strSQL.AppendLine("     , COMP_BUS_DESC              ")
            strSQL.AppendLine("     , PRF_SALE_DESC              ")
            strSQL.AppendLine("         , (CASE WHEN IS_CON_LEASING_COMP = 'Y' THEN '是' ELSE '否' END) ")
            strSQL.AppendLine("              AS IS_CON_LEASING_COMP                                     ")
            strSQL.AppendLine("     , FND_PAY_DESC               ")
            strSQL.AppendLine("     , GUAR_DESC                  ")
            strSQL.AppendLine("     , CONCLUSION                 ")
            strSQL.AppendLine("     , OVERDUE_SAL_PERIOD         ")
            strSQL.AppendLine("         , (CASE WHEN IS_JOIN_SAL_INS = 'Y' THEN 0 ELSE 1 END) IS_JOIN_SAL_INS")
            strSQL.AppendLine("         ,CRD_ID                                ")
            strSQL.AppendLine("         ,FN_GET_USERNM (CRD_ID) AS CRD_NM      ")
            strSQL.AppendLine("         ,VST_CO_ID                             ")
            strSQL.AppendLine("         ,FN_GET_USERNM (VST_CO_ID) AS VST_CO_NM")
            strSQL.AppendLine("         ,FN_C_DATE_VAR2_AC2TW (                                           ")
            strSQL.AppendLine("              TO_CHAR (TO_DATE (RPT_UPLOAD_DT, 'rrrr/mm/dd'), 'yyyymmdd')) ")
            strSQL.AppendLine("              AS RPT_UPLOAD_DT                                             ")
            strSQL.AppendLine("         ,RPT_UPLOAD_DT                                                    ")
            strSQL.AppendLine("     FROM                         ")
            strSQL.AppendLine("         TB_CMLM_CASE_CRDRPT_MAIN ")
            strSQL.AppendLine("     WHERE                        ")
            strSQL.AppendLine("         CASE_SN = :CASE_SN       ")
　
            Return strSQL.ToString()
        End Function
　
#End Region
　
#Region " TB_CMLM_CASE_CRDRPT_PIC "
　
        ''' <summary>
        ''' TB_CMLM_CASE_CRDRPT_PIC
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 1.2014/11/11  Fiona Lin    Create
        ''' </history>
        Public Function GetPicData() As String
　
            Dim strSQL As New System.Text.StringBuilder
　
            strSQL.AppendLine(" SELECT                          ")
            strSQL.AppendLine("           (CASE WHEN PIC_EDU = 'A' THEN 1 ELSE 0 END) AS PIC_EDU_A ")
            strSQL.AppendLine("         , (CASE WHEN PIC_EDU = 'B' THEN 1 ELSE 0 END) AS PIC_EDU_B ")
            strSQL.AppendLine("         , (CASE WHEN PIC_EDU = 'C' THEN 1 ELSE 0 END) AS PIC_EDU_C ")
            strSQL.AppendLine("         , (CASE WHEN PIC_FMY_DESC = 'Y' THEN 1 ELSE 0 END) AS PIC_FMY_DESC_Y ")
            strSQL.AppendLine("         , (CASE WHEN PIC_FMY_DESC = 'N' THEN 1 ELSE 0 END) AS PIC_FMY_DESC_N ")
            strSQL.AppendLine("         , (CASE                                   ")
            strSQL.AppendLine("                 WHEN PIC_FMY_CHILD_DESC = 'True'  ")
            strSQL.AppendLine("                 THEN                              ")
            strSQL.AppendLine("                     '1'                           ")
            strSQL.AppendLine("                 WHEN PIC_FMY_CHILD_DESC = '1'     ")
            strSQL.AppendLine("                 THEN                              ")
            strSQL.AppendLine("                     '1'                           ")
            strSQL.AppendLine("                 WHEN PIC_FMY_CHILD_DESC = 'False' ")
            strSQL.AppendLine("                 THEN                              ")
            strSQL.AppendLine("                     '0'                           ")
            strSQL.AppendLine("                 ELSE                              ")
            strSQL.AppendLine("                     '0'                           ")
            strSQL.AppendLine("             END)                                  ")
            strSQL.AppendLine("              AS PIC_FMY_CHILD_DESC                ")
            strSQL.AppendLine("     , PIC_FMY_CHILD             ")
            strSQL.AppendLine("     , PIC_REALTY_CNT            ")
            strSQL.AppendLine("     , PIC_REALTY_AMT            ")
            strSQL.AppendLine("         , (CASE                                       ")
            strSQL.AppendLine("                 WHEN PIC_IS_PAWN = 'True' THEN '1'    ")
            strSQL.AppendLine("                 WHEN PIC_IS_PAWN = '1' THEN '1'       ")
            strSQL.AppendLine("                 WHEN PIC_IS_PAWN = 'False' THEN '0'   ")
            strSQL.AppendLine("                 ELSE '0'                              ")
            strSQL.AppendLine("             END)                                      ")
            strSQL.AppendLine("              AS PIC_IS_PAWN                           ")
            strSQL.AppendLine("         , (CASE                                       ")
            strSQL.AppendLine("                 WHEN PIC_IS_PAWN_1 = 'True' THEN '1'  ")
            strSQL.AppendLine("                 WHEN PIC_IS_PAWN_1 = '1' THEN '1'     ")
            strSQL.AppendLine("                 WHEN PIC_IS_PAWN_1 = 'False' THEN '0' ")
            strSQL.AppendLine("                 ELSE '0'                              ")
            strSQL.AppendLine("             END)                                      ")
            strSQL.AppendLine("              AS PIC_IS_PAWN_1                         ")
            strSQL.AppendLine("         , (CASE                                       ")
            strSQL.AppendLine("                 WHEN PIC_IS_PAWN_2 = 'True' THEN '1'  ")
            strSQL.AppendLine("                 WHEN PIC_IS_PAWN_2 = '1' THEN '1'     ")
            strSQL.AppendLine("                 WHEN PIC_IS_PAWN_2 = 'False' THEN '0' ")
            strSQL.AppendLine("                 ELSE '0'                              ")
            strSQL.AppendLine("             END)                                      ")
            strSQL.AppendLine("              AS PIC_IS_PAWN_2                         ")
            strSQL.AppendLine("         , (CASE                                       ")
            strSQL.AppendLine("                 WHEN PIC_IS_PAWN_3 = 'True' THEN '1'  ")
            strSQL.AppendLine("                 WHEN PIC_IS_PAWN_3 = '1' THEN '1'     ")
            strSQL.AppendLine("                 WHEN PIC_IS_PAWN_3 = 'False' THEN '0' ")
            strSQL.AppendLine("                 ELSE '0'                              ")
            strSQL.AppendLine("             END)                                      ")
            strSQL.AppendLine("              AS PIC_IS_PAWN_3                         ")
            strSQL.AppendLine("         , (CASE                                       ")
            strSQL.AppendLine("                 WHEN PIC_IS_PAWN_4 = 'True' THEN '1'  ")
            strSQL.AppendLine("                 WHEN PIC_IS_PAWN_4 = '1' THEN '1'     ")
            strSQL.AppendLine("                 WHEN PIC_IS_PAWN_4 = 'False' THEN '0' ")
            strSQL.AppendLine("                 ELSE '0'                              ")
            strSQL.AppendLine("             END)                                      ")
            strSQL.AppendLine("              AS PIC_IS_PAWN_4                         ")
            strSQL.AppendLine("     , PIC_PAWN_AMT1             ")
            strSQL.AppendLine("     , PIC_PAWN_BANK1            ")
            strSQL.AppendLine("     , PIC_PAWN_AMT2             ")
            strSQL.AppendLine("     , PIC_PAWN_BANK2            ")
            strSQL.AppendLine("     , PIC_PAWN_AMT3             ")
            strSQL.AppendLine("     , PIC_PAWN_BANK3            ")
            strSQL.AppendLine("     , PIC_PAWN_AMT4             ")
            strSQL.AppendLine("     , PIC_PAWN_BANK4            ")
            strSQL.AppendLine("         , (CASE WHEN CURR_INDST_YY = '20' THEN 1 ELSE 0 END) AS CURR_INDST_YY_20 ")
            strSQL.AppendLine("         , (CASE WHEN CURR_INDST_YY = '15' THEN 1 ELSE 0 END) AS CURR_INDST_YY_15 ")
            strSQL.AppendLine("         , (CASE WHEN CURR_INDST_YY = '10' THEN 1 ELSE 0 END) AS CURR_INDST_YY_10 ")
            strSQL.AppendLine("         , (CASE WHEN CURR_INDST_YY = '5' THEN 1 ELSE 0 END) AS CURR_INDST_YY_5   ")
            strSQL.AppendLine("         , (CASE WHEN CURR_INDST_YY = '3' THEN 1 ELSE 0 END) AS CURR_INDST_YY_3   ")
            strSQL.AppendLine("         , (CASE WHEN CURR_INDST_YY = 'U3' THEN 1 ELSE 0 END) AS CURR_INDST_YY_U3 ")
            strSQL.AppendLine("         , (CASE WHEN BUS_OUTLOOK = 'G' THEN 1 ELSE 0 END) AS BUS_OUTLOOK_G ")
            strSQL.AppendLine("         , (CASE WHEN BUS_OUTLOOK = 'K' THEN 1 ELSE 0 END) AS BUS_OUTLOOK_K ")
            strSQL.AppendLine("         , (CASE WHEN BUS_OUTLOOK = 'D' THEN 1 ELSE 0 END) AS BUS_OUTLOOK_D ")
            strSQL.AppendLine("     FROM                        ")
            strSQL.AppendLine("         TB_CMLM_CASE_CRDRPT_PIC ")
            strSQL.AppendLine("     WHERE                       ")
            strSQL.AppendLine("         CASE_SN = :CASE_SN      ")
　
            Return strSQL.ToString()
        End Function
　
#End Region
　
#Region " TB_CMLM_CASE_CRDRPT_TRANS "
　
        ''' <summary>
        ''' TB_CMLM_CASE_CRDRPT_TRANS
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 1.2014/11/11  Fiona Lin    Create
        ''' </history>
        Public Function GetTransData() As String
　
            Dim strSQL As New System.Text.StringBuilder
　
            strSQL.AppendLine(" SELECT INSALE_PCT_DESC            ")
            strSQL.AppendLine("         ,'0' AS INSALE_PCT_DESC_1 ")
            strSQL.AppendLine("         ,'0' AS INSALE_PCT_DESC_2 ")
            strSQL.AppendLine("         ,'0' AS INSALE_PCT_DESC_4 ")
            strSQL.AppendLine("     , INSALE_PCT                  ")
            strSQL.AppendLine("     , OUTSALE_PCT                 ")
            strSQL.AppendLine("     , TRADE_PCT                   ")
            strSQL.AppendLine("     , SUPP_NM1                    ")
            strSQL.AppendLine("     , SUPP_NM2                    ")
            strSQL.AppendLine("     , SUPP_NM3                    ")
            strSQL.AppendLine("         ,SUPP_REMIT_PCT_DESC          ")
            strSQL.AppendLine("         ,'0' AS SUPP_REMIT_PCT_DESC_1 ")
            strSQL.AppendLine("         ,'0' AS SUPP_REMIT_PCT_DESC_2 ")
            strSQL.AppendLine("         ,'0' AS SUPP_REMIT_PCT_DESC_4 ")
            strSQL.AppendLine("     , SUPP_REMIT_PCT              ")
            strSQL.AppendLine("     , SUPP_CK_PCT                 ")
            strSQL.AppendLine("     , SUPP_CK_DAY                 ")
            strSQL.AppendLine("     , SUPP_OTH_PCT                ")
            strSQL.AppendLine("     , SALE_NM1                    ")
            strSQL.AppendLine("     , SALE_NM2                    ")
            strSQL.AppendLine("     , SALE_NM3                    ")
            strSQL.AppendLine("         ,SALE_REMIT_PCT_DESC          ")
            strSQL.AppendLine("         ,'0' AS SALE_REMIT_PCT_DESC_1 ")
            strSQL.AppendLine("         ,'0' AS SALE_REMIT_PCT_DESC_2 ")
            strSQL.AppendLine("         ,'0' AS SALE_REMIT_PCT_DESC_4 ")
            strSQL.AppendLine("     , SALE_REMIT_PCT              ")
            strSQL.AppendLine("     , SALE_CK_PCT                 ")
            strSQL.AppendLine("     , SALE_CK_DAY                 ")
            strSQL.AppendLine("     , SALE_OTH_PCT                ")
            strSQL.AppendLine("     FROM                          ")
            strSQL.AppendLine("         TB_CMLM_CASE_CRDRPT_TRANS ")
            strSQL.AppendLine("     WHERE                         ")
            strSQL.AppendLine("         CASE_SN = :CASE_SN        ")
　
            Return strSQL.ToString()
        End Function
　
#End Region
　
#Region " TB_CMLM_CASE_CRDRPT_BANK "
　
        ''' <summary>
        ''' TB_CMLM_CASE_CRDRPT_BANK
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 1.2014/11/11  Fiona Lin    Create
        ''' </history>
        Public Function GetBankData() As String
　
            Dim strSQL As New System.Text.StringBuilder
　
            strSQL.AppendLine(" SELECT  IS_DPST                  ")
            strSQL.AppendLine("         , (CASE WHEN IS_DEPOSIT = 'Y' THEN 1 ELSE 0 END) AS IS_DEPOSIT    ")
            strSQL.AppendLine("         , (CASE WHEN IS_IDN = 'Y' THEN 1 ELSE 0 END) AS IS_IDN            ")
            strSQL.AppendLine("         , (CASE WHEN IS_IDN_OBANK = 'Y' THEN 1 ELSE 0 END) AS IS_IDN_OBANK")
            strSQL.AppendLine("     ,BANK_NM1                    ")
            strSQL.AppendLine("     ,BANK_NM2                    ")
            strSQL.AppendLine("     ,BANK_NM3                    ")
            strSQL.AppendLine("     ,BANK_NM4                    ")
            strSQL.AppendLine("     ,BANK_NM5                    ")
            strSQL.AppendLine("     ,BANK_NM6                    ")
            strSQL.AppendLine("     ,BANK_NM7                    ")
            strSQL.AppendLine("     ,BANK_NM8                    ")
            strSQL.AppendLine("         , (CASE WHEN IS_CK_BEG = 'Y' THEN 1 ELSE 0 END) AS IS_CK_BEG_Y ")
            strSQL.AppendLine("         , (CASE WHEN IS_CK_BEG = 'N' THEN 1 ELSE 0 END) AS IS_CK_BEG_N ")
            strSQL.AppendLine("     FROM                         ")
            strSQL.AppendLine("         TB_CMLM_CASE_CRDRPT_BANK ")
            strSQL.AppendLine("     WHERE                        ")
            strSQL.AppendLine("         CASE_SN = :CASE_SN       ")
　
            Return strSQL.ToString()
        End Function
　
#End Region
　
#Region " TB_CMLM_CASE_CRDRPT_3Y "
　
        ''' <summary>
        ''' TB_CMLM_CASE_CRDRPT_3Y
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 1.2014/11/12  Fiona Lin    Create
        ''' 2.2014/12/20  Fiona Lin    TFBCMLSA-835 增加關係戶5, 6
        ''' </history>
        Public Function Get3YData() As String
　
            Dim strSQL As New System.Text.StringBuilder
　
            strSQL.AppendLine(" SELECT                         ")
            strSQL.AppendLine("         SEQ_NO                 ")
            strSQL.AppendLine("     , YEAR_DESC                ")
            strSQL.AppendLine("     , CUST_AMT                 ")
            strSQL.AppendLine("     , REL1_AMT                 ")
            strSQL.AppendLine("     , REL2_AMT                 ")
            strSQL.AppendLine("     , REL3_AMT                 ")
            strSQL.AppendLine("     , REL4_AMT                 ")
            strSQL.AppendLine("           ,REL5_AMT                ")
            strSQL.AppendLine("           ,REL6_AMT                ")
            strSQL.AppendLine("           , (  NVL (CUST_AMT, 0)   ")
            strSQL.AppendLine("               + NVL (REL1_AMT, 0)  ")
            strSQL.AppendLine("               + NVL (REL2_AMT, 0)  ")
            strSQL.AppendLine("               + NVL (REL3_AMT, 0)  ")
            strSQL.AppendLine("               + NVL (REL4_AMT, 0)  ")
            strSQL.AppendLine("               + NVL (REL5_AMT, 0)  ")
            strSQL.AppendLine("               + NVL (REL6_AMT, 0)) ")
            strSQL.AppendLine("                 AS TOTAL           ")
            strSQL.AppendLine("     FROM                       ")
            strSQL.AppendLine("         TB_CMLM_CASE_CRDRPT_3Y ")
            strSQL.AppendLine("     WHERE                      ")
            strSQL.AppendLine("         CASE_SN = :CASE_SN     ")
            strSQL.AppendLine("       AND ROWNUM  <= 4          ")       '只顯示前面4筆
            strSQL.AppendLine("     ORDER BY SEQ_NO            ")
　
            Return strSQL.ToString()
        End Function
　
#End Region
　
#Region " TB_CMLM_CASE_CRDRPT_LN "
　
        ''' <summary>
        ''' TB_CMLM_CASE_CRDRPT_LN
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 1.2014/11/12  Fiona Lin    Create
        ''' </history>
        Public Function GetLNData() As String
　
            Dim strSQL As New System.Text.StringBuilder
　
            strSQL.AppendLine(" SELECT                         ")
            strSQL.AppendLine("         SEQ_NO                 ")
            strSQL.AppendLine("     , PROJ_ID                  ")
            strSQL.AppendLine("     , PROJ_LN_LMT              ")
            strSQL.AppendLine("     , PROJ_LOAN_LMT            ")
            strSQL.AppendLine("     , PROJ_PERD_LMT            ")
            strSQL.AppendLine("     , DECODE(NVL(TO_CHAR(LOAN_INC_RATE),'A'),'A','', FLOOR(LOAN_INC_RATE * 100 ) || '%') AS LOAN_INC_RATE ")
            strSQL.AppendLine("     , GRD_PERIOD               ")
            strSQL.AppendLine("     FROM                       ")
            strSQL.AppendLine("         TB_CMLM_CASE_CRDRPT_LN ")
            strSQL.AppendLine("     WHERE                      ")
            strSQL.AppendLine("         CASE_SN = :CASE_SN     ")
            strSQL.AppendLine("     ORDER BY SEQ_NO            ")
　
            Return strSQL.ToString()
        End Function
　
#End Region
　
#Region " TB_CMLM_CASE_CRDRPT_PROF "
　
        ''' <summary>
        ''' TB_CMLM_CASE_CRDRPT_PROF
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 1.2014/11/12  Fiona Lin    Create
        ''' 2.2014/12/05  Fiona Lin    TFBCMLSA-760 原本公式計算的地方都改直接計算後塞值
        ''' 3.2014/12/25  Fiona Lin    ACT_GPM是百分比顯示，要再乘100
        ''' </history>
        Public Function GetProfData() As String
　
            Dim strSQL As New System.Text.StringBuilder
　
            strSQL.AppendLine(" SELECT EXP_LOAN                                        ")
            strSQL.AppendLine("         ,EXP_SALMGR                                    ")
            strSQL.AppendLine("         ,ACT_GPM                                       ")
            strSQL.AppendLine("         ,AVG_INC_AMT                                   ")
            strSQL.AppendLine("         ,AVG_DEPOSIT_AMT_3M                            ")
            strSQL.AppendLine("         ,AVG_REV_HALF_YEAR                             ")
            strSQL.AppendLine("         ,AVG_REV_YEAR                                  ")
            strSQL.AppendLine("         ,IS_OVER_AVG_INC_AMT                           ")
            strSQL.AppendLine("         ,NOT_OVER_DESC                                 ")
            strSQL.AppendLine("         , (CASE                                        ")
            strSQL.AppendLine("                 WHEN IS_OVER_AVG_INC_AMT = 'N'         ")
            strSQL.AppendLine("                 THEN                                   ")
            strSQL.AppendLine("                     '實際營收小於損益兩平點，請說明'   ")
            strSQL.AppendLine("                 ELSE                                   ")
            strSQL.AppendLine("                     '實際營收大於損益兩平點'           ")
            strSQL.AppendLine("             END)                                       ")
            strSQL.AppendLine("              AS NOT_OVER_DESC_TITLE                    ")
            strSQL.AppendLine("         , (CASE                                        ")
            strSQL.AppendLine("                 WHEN ACT_GPM IS NULL                   ")
            strSQL.AppendLine("                 THEN                                   ")
            strSQL.AppendLine("                     ''                                 ")
            strSQL.AppendLine("                 ELSE                                   ")
            strSQL.AppendLine("                     TO_CHAR (                          ")
            strSQL.AppendLine("                         ROUND (                        ")
            strSQL.AppendLine("                               (  NVL (EXP_LOAN, 0)     ")
            strSQL.AppendLine("                                 + NVL (EXP_SALARY, 0)  ")
            strSQL.AppendLine("                                 + NVL (EXP_RENT, 0)    ")
            strSQL.AppendLine("                                 + NVL (EXP_SALMGR, 0)) ")
            strSQL.AppendLine("                             * 100                      ")
            strSQL.AppendLine("                             / NVL (ACT_GPM, 1)         ")
            strSQL.AppendLine("                           ,0))                         ")
            strSQL.AppendLine("             END)                                       ")
            strSQL.AppendLine("              AS PL_AMT                                 ")
            strSQL.AppendLine("     FROM                         ")
            strSQL.AppendLine("         TB_CMLM_CASE_CRDRPT_PROF ")
            strSQL.AppendLine("     WHERE                        ")
            strSQL.AppendLine("         CASE_SN = :CASE_SN       ")
　
            Return strSQL.ToString()
        End Function
　
#End Region
　
#Region " TB_CMLM_CASE_CRDRPT_RELCOMP "
　
        ''' <summary>
        ''' TB_CMLM_CASE_CRDRPT_RELCOMP
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 1.2014/11/12  Fiona Lin    Create
        ''' </history>
        Public Function GetRelCompData() As String
　
            Dim strSQL As New System.Text.StringBuilder
　
            strSQL.AppendLine(" SELECT                              ")
            strSQL.AppendLine("         SEQ_NO                      ")
            strSQL.AppendLine("     ,REL_COMP_NM                    ")
            strSQL.AppendLine("     ,REL_BUS_ITEMS                  ")
            strSQL.AppendLine("     ,REL_INVEST_AMT                 ")
            strSQL.AppendLine("     ,REL_TITLE                      ")
            strSQL.AppendLine("     FROM                            ")
            strSQL.AppendLine("         TB_CMLM_CASE_CRDRPT_RELCOMP ")
            strSQL.AppendLine("     WHERE                           ")
            strSQL.AppendLine("         CASE_SN = :CASE_SN          ")
            strSQL.AppendLine("     ORDER BY SEQ_NO                 ")
　
            Return strSQL.ToString()
        End Function
　
#End Region
　
#Region " TB_CMLM_CASE_CRDRPT_SALE "
　
        ''' <summary>
        ''' TB_CMLM_CASE_CRDRPT_SALE
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 1.2014/11/12  Fiona Lin    Create
        ''' </history>
        Public Function GetSaleData() As String
　
            Dim strSQL As New System.Text.StringBuilder
　
            strSQL.AppendLine(" SELECT                           ")
            strSQL.AppendLine("           (CASE WHEN BUS_TYPE = 'A' THEN 1 ELSE 0 END) AS BUS_TYPE_A ")
            strSQL.AppendLine("         , (CASE WHEN BUS_TYPE = 'B' THEN 1 ELSE 0 END) AS BUS_TYPE_B ")
            strSQL.AppendLine("         , (CASE WHEN BUS_TYPE = 'C' THEN 1 ELSE 0 END) AS BUS_TYPE_C ")
            strSQL.AppendLine("         , (CASE WHEN BUS_TYPE = 'D' THEN 1 ELSE 0 END) AS BUS_TYPE_D ")
            strSQL.AppendLine("         , (CASE WHEN BUS_TYPE = 'E' THEN 1 ELSE 0 END) AS BUS_TYPE_E ")
            strSQL.AppendLine("     ,BUS_TYPE_DESC               ")
            strSQL.AppendLine("         , (CASE WHEN BUS_PROD_TYPE = 'A' THEN 1 ELSE 0 END) AS BUS_PROD_TYPE_A ")
            strSQL.AppendLine("         , (CASE WHEN BUS_PROD_TYPE = 'H' THEN 1 ELSE 0 END) AS BUS_PROD_TYPE_H ")
            strSQL.AppendLine("         , (CASE WHEN BUS_PLACE_TYPE = 'A' THEN 1 ELSE 0 END) AS BUS_PLACE_TYPE_A ")
            strSQL.AppendLine("         , (CASE WHEN BUS_PLACE_TYPE = 'B' THEN 1 ELSE 0 END) AS BUS_PLACE_TYPE_B ")
            strSQL.AppendLine("         , (CASE WHEN BUS_PLACE_STATUS = 'C' THEN 1 ELSE 0 END) AS BUS_PLACE_STATUS_C ")
            strSQL.AppendLine("         , (CASE WHEN BUS_PLACE_STATUS = 'D' THEN 1 ELSE 0 END) AS BUS_PLACE_STATUS_D ")
            strSQL.AppendLine("     ,BUS_PLACE_RENT              ")
            strSQL.AppendLine("     ,BUS_EMPLOYEES               ")
            strSQL.AppendLine("     ,BUS_MM_SALARY               ")
            strSQL.AppendLine("     ,BUS_EQUI_COST               ")
            strSQL.AppendLine("     ,BUS_AREA                    ")
            strSQL.AppendLine("         , (CASE WHEN BUS_AREA_INSU = 'Y' THEN 1 ELSE 0 END) AS BUS_AREA_INSU_Y ")
            strSQL.AppendLine("         , (CASE WHEN BUS_AREA_INSU = 'N' THEN 1 ELSE 0 END) AS BUS_AREA_INSU_N ")
            strSQL.AppendLine("     ,BUS_AREA_INSU_AMT           ")
            strSQL.AppendLine("     ,BUS_STCK                    ")
            strSQL.AppendLine("     ,BUS_SAFE_STCK               ")
            strSQL.AppendLine("         , (CASE WHEN BUS_STCK_INSU = 'Y' THEN 1 ELSE 0 END) AS BUS_STCK_INSU_Y ")
            strSQL.AppendLine("         , (CASE WHEN BUS_STCK_INSU = 'N' THEN 1 ELSE 0 END) AS BUS_STCK_INSU_N ")
            strSQL.AppendLine("     ,BUS_STCK_INSU_AMT           ")
            strSQL.AppendLine("         ,BUS_FND_SOURCE          ")
            strSQL.AppendLine("         ,BUS_FND_SOURCE          ")
            strSQL.AppendLine("         ,'0' AS BUS_FND_SOURCE_1 ")
            strSQL.AppendLine("         ,'0' AS BUS_FND_SOURCE_2 ")
            strSQL.AppendLine("         ,'0' AS BUS_FND_SOURCE_4 ")
            strSQL.AppendLine("         ,'0' AS BUS_FND_SOURCE_8 ")
            strSQL.AppendLine("         ,'0' AS BUS_FND_SOURCE_16")
            strSQL.AppendLine("     FROM                         ")
            strSQL.AppendLine("         TB_CMLM_CASE_CRDRPT_SALE ")
            strSQL.AppendLine("     WHERE                        ")
            strSQL.AppendLine("         CASE_SN = :CASE_SN       ")
　
            Return strSQL.ToString()
        End Function
　
#End Region
　
#Region " TB_CMLM_CASE_CRDRPT_UGR "
　
        ''' <summary>
        ''' TB_CMLM_CASE_CRDRPT_UGR
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 1.2014/11/12  Fiona Lin    Create
        ''' </history>
        Public Function GetUgrData() As String
　
            Dim strSQL As New System.Text.StringBuilder
　
            strSQL.AppendLine(" SELECT M3_AMT                                                      ")
            strSQL.AppendLine("         ,LAW_CHK                                                   ")
            strSQL.AppendLine("         , (CASE WHEN LAW_CHK = 'Y' THEN 1 ELSE 0 END) AS LAW_CHK_1 ")
            strSQL.AppendLine("         , (CASE WHEN LAW_CHK = 'N' THEN 1 ELSE 0 END) AS LAW_CHK_2 ")
            strSQL.AppendLine("     FROM                        ")
            strSQL.AppendLine("         TB_CMLM_CASE_CRDRPT_UGR ")
            strSQL.AppendLine("     WHERE                       ")
            strSQL.AppendLine("         CASE_SN = :CASE_SN      ")
　
            Return strSQL.ToString()
        End Function
　
#End Region
　
#Region "TB_CMLM_CASE_LINECOND"
        ''' <summary>
        ''' TB_CMLM_CASE_LINECOND
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 1.2015/09/08  Jessifer Nien    Create
        ''' </history>
        Public Function GetLineCondData() As String
　
            Dim strSQL As New System.Text.StringBuilder
　
            strSQL.Append(" SELECT COM_DESC ")
            strSQL.Append(" FROM TB_CMLM_CASE_LINECOND ")
            strSQL.Append(" WHERE CASE_SN=:CASE_SN  ")
　
            Return strSQL.ToString()
        End Function
#End Region
　
#Region " GetCaseLineCustProcCount "
　
        ''' <summary>
        ''' GetCaseLineCustProcCount
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 1.2014/12/19  Fiona Lin    TFBCMLSA-844 Create
        ''' </history>
        Public Function GetCaseLineCustProcCount() As String
　
            Dim strSQL As New System.Text.StringBuilder
　
            strSQL.AppendLine(" SELECT COUNT (1) AS COUNT        ")
            strSQL.AppendLine("   FROM TB_CMLM_CASE_LINECUST_REL ")
            strSQL.AppendLine("  WHERE CASE_SN = :CASE_SN        ")
            strSQL.AppendLine("     AND PROC_STEP = 'C'          ")
　
            Return strSQL.ToString()
        End Function
　
#End Region
　
#Region " GetDataYM_A "
　
        ''' <summary>
        ''' GetDataYM_A
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 1.2014/12/19  Fiona Lin    TFBCMLSA-844 Create
        ''' 2.2014/12/20  Fiona Lin    TFBCMLSA-844 加入 CUST_ID條件
        ''' </history>
        Public Function GetDataYM_A() As String
　
            Dim strSQL As New System.Text.StringBuilder
　
            strSQL.AppendLine("   SELECT DATA_YM                     ")
            strSQL.AppendLine("      FROM TB_CMLM_CASE_BAL_BANK_INFO ")
            strSQL.AppendLine("     WHERE CASE_SN = :CASE_SN         ")
            strSQL.AppendLine("       AND CUST_ID = (SELECT CUST_ID                  ")
            strSQL.AppendLine("                              FROM TB_CMLM_CASE_MAIN  ")
            strSQL.AppendLine("                             WHERE CASE_SN = :CASE_SN)")
            strSQL.AppendLine(" GROUP BY DATA_YM                     ")
            strSQL.AppendLine(" ORDER BY DATA_YM                     ")
　
            Return strSQL.ToString()
        End Function
　
#End Region
　
#Region " Get3YDebtList "
　
        ''' <summary>
        ''' Get3YDebtList
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 1.2014/12/10  Fiona Lin    Create
        ''' 2.2014/12/19  Fiona Lin    TFBCMLSA-844 需求變更 行列轉置
        ''' 3.2014/12/24  Fiona Lin    僅顯示有聯徵查詢過的人員, TFBCMLSA-895 顯示年度增加一年Y5, 為null時須顯示0
        ''' </history>
        Public Function Get3YDebtList(ByVal dataYMList As String) As String
　
            Dim strSQL As New System.Text.StringBuilder
　
            strSQL.AppendLine("   SELECT CASE                                                                              ")
            strSQL.AppendLine("                 WHEN IDENT IN ('004', '005')                                               ")
            strSQL.AppendLine("                 THEN                                                                       ")
            strSQL.AppendLine("                         FN_GET_CODENM ('110', IDENT)                                       ")
            strSQL.AppendLine("                     || '('                                                                 ")
            strSQL.AppendLine("                     || (SELECT CUST_NM                                                     ")
            strSQL.AppendLine("                             FROM TB_CMLM_CASE_CUST CUST                                    ")
            strSQL.AppendLine("                           WHERE CUST.CASE_SN = :CASE_SN                                    ")   'CASE_SN
            strSQL.AppendLine("                              AND CUST.CUST_ID = A.CUST_ID)                                 ")
            strSQL.AppendLine("                     || ')'                                                                 ")
            strSQL.AppendLine("                 ELSE                                                                       ")
            strSQL.AppendLine("                     FN_GET_CODENM ('110', IDENT)                                           ")
            strSQL.AppendLine("             END                                                                            ")
            strSQL.AppendLine("                 AS TITLE                                                                   ")
            strSQL.AppendLine("           ,NVL (Y1_DEBTS, 0) Y1_DEBTS ")
            strSQL.AppendLine("           ,NVL (Y2_DEBTS, 0) Y2_DEBTS ")
            strSQL.AppendLine("           ,NVL (Y3_DEBTS, 0) Y3_DEBTS ")
            strSQL.AppendLine("           ,NVL (Y4_DEBTS, 0) Y4_DEBTS ")
            strSQL.AppendLine("           ,NVL (Y5_DEBTS, 0) Y5_DEBTS ")
            strSQL.AppendLine("      FROM (SELECT CASE_SN    ")
            strSQL.AppendLine("                      ,CUST_ID")
            strSQL.AppendLine("                      ,CASE                                                                 ")
            strSQL.AppendLine("                           WHEN RELATION = '002'                                            ")
            strSQL.AppendLine("                           THEN                                                             ")
            strSQL.AppendLine("                               '002'                                                        ")
            strSQL.AppendLine("                           WHEN RELATION = '003'                                            ")
            strSQL.AppendLine("                           THEN                                                             ")
            strSQL.AppendLine("                               '003'                                                        ")
            strSQL.AppendLine("                           ELSE                                                             ")
            strSQL.AppendLine("                               IDENT_TYPE                                                   ")
            strSQL.AppendLine("                       END                                                                  ")
            strSQL.AppendLine("                           AS IDENT                                                         ")
            strSQL.AppendLine("                 FROM TB_CMLM_CASE_CUST_IDENT IDENT                                         ")
            strSQL.AppendLine("               WHERE CASE_SN = :CASE_SN                                                     ")   'CASE_SN
            strSQL.AppendLine("                  AND (IDENT_TYPE IN ('001', '005') ")
            strSQL.AppendLine("                     OR RELATION IN ('002', '003')) ")
            strSQL.AppendLine("              UNION                                                                         ")
            strSQL.AppendLine("              SELECT CASE_SN  ")
            strSQL.AppendLine("                      ,CUST_ID")
            strSQL.AppendLine("                      ,CASE                                                                 ")
            strSQL.AppendLine("                           WHEN RELATION = '002'                                            ")
            strSQL.AppendLine("                           THEN                                                             ")
            strSQL.AppendLine("                               '002'                                                        ")
            strSQL.AppendLine("                           WHEN RELATION = '003'                                            ")
            strSQL.AppendLine("                           THEN                                                             ")
            strSQL.AppendLine("                               '003'                                                        ")
            strSQL.AppendLine("                           ELSE                                                             ")
            strSQL.AppendLine("                               IDENT_TYPE                                                   ")
            strSQL.AppendLine("                       END                                                                  ")
            strSQL.AppendLine("                           AS IDENT                                                         ")
            strSQL.AppendLine("                 FROM TB_CMLM_CASE_CUST_IDENT IDNT                                          ")
            strSQL.AppendLine("               WHERE CASE_SN = :CASE_SN                                                     ")   'CASE_SN
            strSQL.AppendLine("                  AND IDENT_TYPE = '004'                                                    ")
            strSQL.AppendLine("                  AND EXISTS                                                                ")
            strSQL.AppendLine("                           (SELECT 1                                                        ")
            strSQL.AppendLine("                               FROM TB_CMLM_CASE_LINECUST_REL REL                           ")
            strSQL.AppendLine("                              WHERE IDNT.CASE_SN = REL.CASE_SN                              ")
            strSQL.AppendLine("                                 AND IDNT.CUST_ID = REL.CUST_ID                             ")
            strSQL.AppendLine("                                 AND REL.PROC_STEP = :PROC_STEP)) A                         ")   'PROC_STEP
            strSQL.AppendLine("             INNER JOIN                                                                     ")
            strSQL.AppendLine("             (SELECT *                                                                      ")
            strSQL.AppendLine("                 FROM (SELECT A.CUST_ID, A.DATA_YM, A.NORMAL_LOAN_AMT                       ")
            strSQL.AppendLine("                           FROM TB_CMLM_CASE_BAL_BANK_INFO A                                ")
            strSQL.AppendLine("                          WHERE A.CASE_SN = :CASE_SN) PIVOT (SUM (NORMAL_LOAN_AMT) AS DEBTS ")   'CASE_SN
            strSQL.AppendLine("                                                               FOR DATA_YM                  ")
            strSQL.AppendLine("                                                               IN   (" & dataYMList & "))) B")
            strSQL.AppendLine("                 ON A.CUST_ID = B.CUST_ID       ")
            strSQL.AppendLine("     WHERE EXISTS                               ")
            strSQL.AppendLine("                 (SELECT DISTINCT CUST_ID       ")
            strSQL.AppendLine("                     FROM LMCM_JCIC_QUERY ")
            strSQL.AppendLine("                   WHERE CASE_SN = A.CASE_SN    ")
            strSQL.AppendLine("                      AND CUST_ID = A.CUST_ID   ")
            strSQL.AppendLine("                      AND STEP_ID = 'ALL')      ")
            strSQL.AppendLine(" ORDER BY A.IDENT                                                                           ")
　
            Return strSQL.ToString()
        End Function
　
#End Region
　
#Region " GetCaseCustId_B "
　
        ''' <summary>
        ''' GetCaseCustId_B
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 1.2014/12/10  Fiona Lin    Create
        ''' 2.2012/12/20  Fiona Lin    TFBCMLSA-844 B, C區標題從代碼檔取才會一致
        ''' 3.2014/12/24  Fiona Lin    僅顯示有聯徵查詢過的人員
        ''' </history>
        Public Function GetCaseCustId_B() As String
　
            Dim strSQL As New System.Text.StringBuilder
　
            strSQL.AppendLine("   SELECT CUST_ID AS BANK_LOAN_CUST_ID                                         ")
            strSQL.AppendLine("           ,IDENT_TYPE                                                         ")
            strSQL.AppendLine("           , (CASE IDENT_TYPE                                ")
            strSQL.AppendLine("                   WHEN '001'                                ")
            strSQL.AppendLine("                   THEN                                      ")
            strSQL.AppendLine("                       FN_GET_CODENM ('110', IDENT_TYPE)     ")
            strSQL.AppendLine("                   ELSE                                      ")
            strSQL.AppendLine("                           FN_GET_CODENM ('110', IDENT_TYPE) ")
            strSQL.AppendLine("                       || '('                                ")
            strSQL.AppendLine("                       || TO_CHAR (FN_GET_CASE_CUSTNM (CASE_SN,CUST_ID))  ")
            strSQL.AppendLine("                       || ')'                                ")
            strSQL.AppendLine("               END)                                          ")
            strSQL.AppendLine("                 BANK_LOAN_CUST_NM                           ")
            strSQL.AppendLine("      FROM TB_CMLM_CASE_CUST_IDENT A                                           ")
            strSQL.AppendLine("     WHERE CASE_SN = :CASE_SN                                                  ")
            strSQL.AppendLine("       AND IDENT_TYPE IN ('001', '005')                                        ")
            strSQL.AppendLine("       AND EXISTS                               ")
            strSQL.AppendLine("                 (SELECT DISTINCT CUST_ID       ")
            strSQL.AppendLine("                     FROM LMCM_JCIC_QUERY ")
            strSQL.AppendLine("                   WHERE CASE_SN = A.CASE_SN    ")
            strSQL.AppendLine("                      AND CUST_ID = A.CUST_ID   ")
            strSQL.AppendLine("                      AND STEP_ID = 'ALL' )      ")
            strSQL.AppendLine(" ORDER BY IDENT_TYPE, CUST_ID                                                  ")
　
            Return strSQL.ToString()
        End Function
　
#End Region
　
#Region " GetBankLoanSummaryList "
　
        ''' <summary>
        ''' GetBankLoanSummaryList
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 1.2014/12/10  Fiona Lin    Create
        ''' </history>
        Public Function GetBankLoanSummaryList() As String
　
            Dim strSQL As New System.Text.StringBuilder
　
            strSQL.AppendLine("   SELECT A.CASE_SN                                                   ")
            strSQL.AppendLine("           ,A.CUST_ID                                                 ")
            strSQL.AppendLine("           ,BANK_ID                                                   ")
            strSQL.AppendLine("           ,BNKCCSELN.FN_LM_GET_CODE('097',A.BANK_ID,'') AS BANK_NM   ")
            strSQL.AppendLine("           ,FN_GET_DEBTS_INFO (A.CASE_SN                              ")
            strSQL.AppendLine("                                     ,A.CUST_ID                       ")
            strSQL.AppendLine("                                     ,BANK_ID                         ")
            strSQL.AppendLine("                                     ,CONTRACT_CODE1                  ")
            strSQL.AppendLine("                                     ,ACCOUNT_CODE                    ")
            strSQL.AppendLine("                                     ,'1')                            ")
            strSQL.AppendLine("                 AS ACT                                               ")
            strSQL.AppendLine("           ,SUM (LOAN_LN) AS LOAN_LN                                  ")
            strSQL.AppendLine("           ,SUM (BAL_AMT) AS BAL_AMT                                  ")
            strSQL.AppendLine("           ,FN_GET_DEBTS_INFO (A.CASE_SN                              ")
            strSQL.AppendLine("                                     ,A.CUST_ID                       ")
            strSQL.AppendLine("                                     ,BANK_ID                         ")
            strSQL.AppendLine("                                     ,CONTRACT_CODE1                  ")
            strSQL.AppendLine("                                     ,ACCOUNT_CODE                    ")
            strSQL.AppendLine("                                     ,'2')                            ")
            strSQL.AppendLine("                 AS KIND                                              ")
            strSQL.AppendLine("           ,A.CONTRACT_CODE1                                          ")
            strSQL.AppendLine("      FROM TB_CMLM_CASE_BANK_INFO A                                   ")
            strSQL.AppendLine("     WHERE A.CASE_SN = :CASE_SN                                       ")
            strSQL.AppendLine("       AND A.CUST_ID = :CUST_ID                                       ")
            strSQL.AppendLine("       AND A.CONTRACT_CODE1 = LPAD ('9', 50, '9')                     ")
            'strSQL.AppendLine("       AND A.ACCOUNT_CODE IN ('E', 'H', 'I')                          ")
            strSQL.AppendLine(" GROUP BY A.CASE_SN                                                   ")
            strSQL.AppendLine("           ,A.CUST_ID                                                 ")
            strSQL.AppendLine("           ,A.BANK_ID                                                 ")
            strSQL.AppendLine("           ,A.CONTRACT_CODE1                                          ")
            strSQL.AppendLine("           ,A.ACCOUNT_CODE                                            ")
            strSQL.AppendLine(" UNION                                                                ")
            strSQL.AppendLine("   SELECT A.CASE_SN                                                   ")
            strSQL.AppendLine("           ,A.CUST_ID                                                 ")
            strSQL.AppendLine("           ,BANK_ID                                                   ")
            strSQL.AppendLine("           ,BNKCCSELN.FN_LM_GET_CODE('097',A.BANK_ID,'') AS BANK_NM   ")
            strSQL.AppendLine("           ,FN_GET_DEBTS_INFO (A.CASE_SN                              ")
            strSQL.AppendLine("                                     ,A.CUST_ID                       ")
            strSQL.AppendLine("                                     ,BANK_ID                         ")
            strSQL.AppendLine("                                     ,CONTRACT_CODE1                  ")
            strSQL.AppendLine("                                     ,''                              ")
            strSQL.AppendLine("                                     ,'1')                            ")
            strSQL.AppendLine("                 AS ACT                                               ")
            strSQL.AppendLine("           ,MAX (LOAN_LN) AS LOAN_LN                                  ")
            strSQL.AppendLine("           ,SUM (BAL_AMT) AS BAL_AMT                                  ")
            strSQL.AppendLine("           ,FN_GET_DEBTS_INFO (A.CASE_SN                              ")
            strSQL.AppendLine("                                     ,A.CUST_ID                       ")
            strSQL.AppendLine("                                     ,BANK_ID                         ")
            strSQL.AppendLine("                                     ,CONTRACT_CODE1                  ")
            strSQL.AppendLine("                                     ,''                              ")
            strSQL.AppendLine("                                     ,'2')                            ")
            strSQL.AppendLine("                 AS KIND                                              ")
            strSQL.AppendLine("           ,A.CONTRACT_CODE1                                          ")
            strSQL.AppendLine("      FROM TB_CMLM_CASE_BANK_INFO A                                   ")
            strSQL.AppendLine("     WHERE A.CASE_SN = :CASE_SN                                       ")
            strSQL.AppendLine("       AND A.CUST_ID = :CUST_ID                                       ")
            strSQL.AppendLine("       AND A.CONTRACT_CODE1 <> LPAD ('9', 50, '9')                    ")
            strSQL.AppendLine(" GROUP BY A.CASE_SN                                                   ")
            strSQL.AppendLine("           ,A.CUST_ID                                                 ")
            strSQL.AppendLine("           ,A.BANK_ID                                                 ")
            strSQL.AppendLine("           ,A.CONTRACT_CODE1                                          ")
            strSQL.AppendLine(" ORDER BY BANK_ID                                                     ")
　
            Return strSQL.ToString()
        End Function
　
#End Region
　
#Region " GetCaseCustId_C "
　
        ''' <summary>
        ''' GetCaseCustId_C
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 1.2014/12/11  Fiona Lin    Create
        ''' 2.2014/12/19  Fiona Lin    TFBCMLSA-844 增加B區UNION
        ''' 3.2014/12/20  Fiona Lin    TFBCMLSA-844 B, C區標題從代碼檔取才會一致
        ''' 4.2014/12/24  Fiona Lin    僅顯示有聯徵查詢過的人員
        ''' 5.2014/12/24  Fiona Lin    調整撈取邏輯
        ''' </history>
        Public Function GetCaseCustId_C() As String
　
            Dim strSQL As New System.Text.StringBuilder
　
            strSQL.AppendLine("   SELECT CUST_ID AS RELATION_CUST_ID                                 ")
            strSQL.AppendLine("           ,IDENT_TYPE                                                ")
            strSQL.AppendLine("           , (CASE IDENT_TYPE                                ")
            strSQL.AppendLine("                   WHEN '002'                                ")
            strSQL.AppendLine("                   THEN                                      ")
            strSQL.AppendLine("                       FN_GET_CODENM ('110', IDENT_TYPE)     ")
            strSQL.AppendLine("                   WHEN '003'                                ")
            strSQL.AppendLine("                   THEN                                      ")
            strSQL.AppendLine("                       FN_GET_CODENM ('110', IDENT_TYPE)     ")
            strSQL.AppendLine("                   ELSE                                      ")
            strSQL.AppendLine("                           FN_GET_CODENM ('110', IDENT_TYPE) ")
            strSQL.AppendLine("                       || '('                                ")
            strSQL.AppendLine("                       || TO_CHAR (FN_GET_CASE_CUSTNM (CASE_SN,CUST_ID))  ")
            strSQL.AppendLine("                       || ')'                                ")
            strSQL.AppendLine("               END)                                          ")
            strSQL.AppendLine("                 RELATION_CUST_NM                            ")
            strSQL.AppendLine("      FROM (SELECT CASE_SN,CUST_ID, RELATION AS IDENT_TYPE                    ")
            strSQL.AppendLine("                 FROM TB_CMLM_CASE_CUST_IDENT                         ")
            strSQL.AppendLine("               WHERE CASE_SN = :CASE_SN                               ")
            strSQL.AppendLine("                  AND (RELATION IN ('002', '003'))                    ")
            strSQL.AppendLine("              UNION                                                   ")
            strSQL.AppendLine("              (SELECT CASE_SN,CUST_ID                                         ")
            strSQL.AppendLine("                       ,CASE                                          ")
            strSQL.AppendLine("                             WHEN RELATION IN ('002', '003')          ")
            strSQL.AppendLine("                             THEN                                     ")
            strSQL.AppendLine("                                 RELATION                             ")
            strSQL.AppendLine("                             ELSE                                     ")
            strSQL.AppendLine("                                 IDENT_TYPE                           ")
            strSQL.AppendLine("                         END                                          ")
            strSQL.AppendLine("                  FROM TB_CMLM_CASE_CUST_IDENT IDNT                   ")
            strSQL.AppendLine("                 WHERE CASE_SN = :CASE_SN                             ")
            strSQL.AppendLine("                   AND IDENT_TYPE = '004'                             ")
            strSQL.AppendLine("                   AND EXISTS                                         ")
            strSQL.AppendLine("                             (SELECT 1                                ")
            strSQL.AppendLine("                                 FROM TB_CMLM_CASE_LINECUST_REL REL   ")
            strSQL.AppendLine("                               WHERE IDNT.CASE_SN = REL.CASE_SN       ")
            strSQL.AppendLine("                                  AND IDNT.CUST_ID = REL.CUST_ID      ")
            strSQL.AppendLine("                                  AND REL.PROC_STEP = :PROC_STEP)     ")
            strSQL.AppendLine("                   AND EXISTS                                         ")
            strSQL.AppendLine("                             (SELECT DISTINCT CUST_ID                 ")
            strSQL.AppendLine("                                 FROM TB_CMLM_CASE_QRY_MAIN           ")
            strSQL.AppendLine("                               WHERE CASE_SN = IDNT.CASE_SN           ")
            strSQL.AppendLine("                                  AND CUST_ID = IDNT.CUST_ID          ")
            strSQL.AppendLine("                                  AND STEP_ID = 'ALL'))) A            ")
            strSQL.AppendLine(" ORDER BY IDENT_TYPE                                                  ")
　
            Return strSQL.ToString()
        End Function
　
#End Region
　
#Region " GetBankLoanRelationList "
　
        ''' <summary>
        ''' GetBankLoanRelationList
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 1.2014/12/10  Fiona Lin    Create
        ''' 2.2014/12/24  Fiona Lin    調整loan計算
        ''' </history>
        Public Function GetBankLoanRelationList() As String
　
            Dim strSQL As New System.Text.StringBuilder
　
            strSQL.AppendLine("   SELECT A.CASE_SN                                                   ")
            strSQL.AppendLine("           ,A.CUST_ID                                                 ")
            strSQL.AppendLine("           ,BANK_ID                                                   ")
            strSQL.AppendLine("           ,BNKCCSELN.FN_LM_GET_CODE('097',A.BANK_ID,'') AS BANK_NM   ")
            strSQL.AppendLine("           ,FN_GET_DEBTS_INFO (A.CASE_SN                              ")
            strSQL.AppendLine("                                     ,A.CUST_ID                       ")
            strSQL.AppendLine("                                     ,BANK_ID                         ")
            strSQL.AppendLine("                                     ,CONTRACT_CODE1                  ")
            strSQL.AppendLine("                                     ,ACCOUNT_CODE                    ")
            strSQL.AppendLine("                                     ,'1')                            ")
            strSQL.AppendLine("                 AS ACT                                               ")
            strSQL.AppendLine("           , (CASE                           ")
            strSQL.AppendLine("                   WHEN A.ACCOUNT_CODE = 'Y' ")
            strSQL.AppendLine("                   THEN                      ")
            strSQL.AppendLine("                       SUM (CONTRACT_AMT_Y)  ")
            strSQL.AppendLine("                   ELSE                      ")
            strSQL.AppendLine("                       SUM (LOAN_LN)         ")
            strSQL.AppendLine("               END)                          ")
            strSQL.AppendLine("                 AS LOAN_LN                  ")
            strSQL.AppendLine("           ,SUM (BAL_AMT) AS BAL_AMT                                  ")
            strSQL.AppendLine("           ,FN_GET_DEBTS_INFO (A.CASE_SN                              ")
            strSQL.AppendLine("                                     ,A.CUST_ID                       ")
            strSQL.AppendLine("                                     ,BANK_ID                         ")
            strSQL.AppendLine("                                     ,CONTRACT_CODE1                  ")
            strSQL.AppendLine("                                     ,ACCOUNT_CODE                    ")
            strSQL.AppendLine("                                     ,'2')                            ")
            strSQL.AppendLine("                 AS KIND                                              ")
            strSQL.AppendLine("           ,A.CONTRACT_CODE1                                          ")
            strSQL.AppendLine("      FROM TB_CMLM_CASE_BANK_INFO A                                   ")
            strSQL.AppendLine("     WHERE A.CASE_SN = :CASE_SN                                       ")
            strSQL.AppendLine("       AND A.CUST_ID = :CUST_ID                                       ")
            strSQL.AppendLine("       AND A.CONTRACT_CODE1 = LPAD ('9', 50, '9')                     ")
            'strSQL.AppendLine("       AND A.ACCOUNT_CODE IN ('E', 'H', 'I','Y')                      ")
            strSQL.AppendLine(" GROUP BY A.CASE_SN                                                   ")
            strSQL.AppendLine("           ,A.CUST_ID                                                 ")
            strSQL.AppendLine("           ,A.BANK_ID                                                 ")
            strSQL.AppendLine("           ,A.CONTRACT_CODE1                                          ")
            strSQL.AppendLine("           ,A.ACCOUNT_CODE                                            ")
            strSQL.AppendLine(" UNION                                                                ")
            strSQL.AppendLine("   SELECT A.CASE_SN                                                   ")
            strSQL.AppendLine("           ,A.CUST_ID                                                 ")
            strSQL.AppendLine("           ,BANK_ID                                                   ")
            strSQL.AppendLine("           ,BNKCCSELN.FN_LM_GET_CODE('097',A.BANK_ID,'') AS BANK_NM   ")
            strSQL.AppendLine("           ,FN_GET_DEBTS_INFO (A.CASE_SN                              ")
            strSQL.AppendLine("                                     ,A.CUST_ID                       ")
            strSQL.AppendLine("                                     ,BANK_ID                         ")
            strSQL.AppendLine("                                     ,CONTRACT_CODE1                  ")
            strSQL.AppendLine("                                     ,''                              ")
            strSQL.AppendLine("                                     ,'1')                            ")
            strSQL.AppendLine("                 AS ACT                                               ")
            strSQL.AppendLine("           ,MAX (LOAN_LN) AS LOAN_LN                                  ")
            strSQL.AppendLine("           ,SUM (BAL_AMT) AS BAL_AMT                                  ")
            strSQL.AppendLine("           ,FN_GET_DEBTS_INFO (A.CASE_SN                              ")
            strSQL.AppendLine("                                     ,A.CUST_ID                       ")
            strSQL.AppendLine("                                     ,BANK_ID                         ")
            strSQL.AppendLine("                                     ,CONTRACT_CODE1                  ")
            strSQL.AppendLine("                                     ,''                              ")
            strSQL.AppendLine("                                     ,'2')                            ")
            strSQL.AppendLine("                 AS KIND                                              ")
            strSQL.AppendLine("           ,A.CONTRACT_CODE1                                          ")
            strSQL.AppendLine("      FROM TB_CMLM_CASE_BANK_INFO A                                   ")
            strSQL.AppendLine("     WHERE A.CASE_SN = :CASE_SN                                       ")
            strSQL.AppendLine("       AND A.CUST_ID = :CUST_ID                                       ")
            strSQL.AppendLine("       AND A.CONTRACT_CODE1 <> LPAD ('9', 50, '9')                    ")
            strSQL.AppendLine(" GROUP BY A.CASE_SN                                                   ")
            strSQL.AppendLine("           ,A.CUST_ID                                                 ")
            strSQL.AppendLine("           ,A.BANK_ID                                                 ")
            strSQL.AppendLine("           ,A.CONTRACT_CODE1                                          ")
            strSQL.AppendLine(" ORDER BY BANK_ID                                                     ")
　
            Return strSQL.ToString()
        End Function
　
#End Region
　
#Region " GetSummaryAmt "
　
        ''' <summary>
        ''' GetSummaryAmt
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <history>
        ''' 1.2014/12/11  Fiona Lin    Create
        ''' </history>
        Public Function GetSummaryAmt() As String
　
            Dim strSQL As New System.Text.StringBuilder
　
            strSQL.AppendLine(" SELECT (NVL (REVOL_AMT, 0) / 1000) AS REVOL_AMT ")
            strSQL.AppendLine("         ,NVL (S_LOAN_AMT, 0) AS S_LOAN_AMT      ")
            strSQL.AppendLine("   FROM TB_CMLM_CASE_JCIC_IMPORT                 ")
            strSQL.AppendLine("  WHERE CASE_SN = :CASE_SN                       ")
            strSQL.AppendLine("     AND CUST_ID = :CUST_ID                      ")
　
            Return strSQL.ToString()
        End Function
　
#End Region
　
#End Region
　
#Region " Insert "
　
#End Region
　
#Region " Delete "
　
#End Region
　
#Region " Update "
　
        ''' <summary>
        ''' 更新案件表單檔狀態TB_CMLM_CASE_FORM
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function UpdateCaseForm() As String
　
            Dim strSql As New System.Text.StringBuilder
　
            strSql.AppendLine("UPDATE TB_CMLM_CASE_FORM                   ")
            strSql.AppendLine(" SET                                       ")
            strSql.AppendLine("  RPT_NM = :RPT_NM                         ")
            strSql.AppendLine(" , RPT_PATH = FN_GET_PATH('FORM',:CASE_SN) ")
            strSql.AppendLine(" , UPD_USERID = :UPD_USERID                ")
            strSql.AppendLine(" , UPD_DT     = sysdate                    ")
            strSql.AppendLine(" WHERE                                     ")
            strSql.AppendLine("  CASE_SN = :CASE_SN                       ")
            strSql.AppendLine("  AND TRIM(FORM_ID) = :FORM_ID             ")
　
            Return strSql.ToString()
        End Function
　
#End Region
　
#End Region
　
#Region "Private Method"
　
#End Region
　
    End Class
End Namespace