   M S B A M L   `   `   `  ����F  BTic_Tac_Toe, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:Tic_Tac_ToeTic_Tac_Toe  5   	       Tic_Tac_Toe.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:Tic_Tac_Toe     5   	   -   5          � Title$  Tic-Tac-Toe��5   	   $��414.078��6   $��508.296��6.   .��5	      � .+�5
   
   �� -   5         ��Name$ txtDestinationIP��5
      $
��Left=�6,   $��23��6G   $��
10,139,0,0q�6S    �� TextWrapping$
 Wrap=�6g   $	��Top=�6{   $	��138��6�   $��	127.0.0.1��6�   �� 5   
   $��Opponent IP ��6   $
��Left=�6'   $��
10,110,0,0q�6B   $	��Top=�6V   �� 5   
   -   5        $ txtDestinationPort��5      $
��Left=�6.   $��23��6I   $��
10,198,0,0q�6U   $
 Wrap=�6i   $	��Top=�6}   $	��138��6�   $��50000��6�   �� 5   
   $��Opponent Port��6   $
��Left=�6(   $��
10,167,0,0q�6C   $	��Top=�6W   �� 5   
   -   5        $ txtSourcePort��5      $
��Left=�6)   $��23��6D   $��	10,36,0,0q�6P   $
 Wrap=�6c   $��50000��6w   $	��Top=�6�   $	��138��6�   �� 5   
   $��Source Port��6   $
��Left=�6&   $��	10,10,0,0q�6A   $	��Top=�6T   �� 5   
   -   5         ��Name$ 
btnConnect��5      $��Connect to host��6%   $
��Left=�6?   $��
10,236,0,0q�6Z   $	��Top=�6n   $��96��6�   6�   �� 5   
   -   5        $ btnHost��5      $��	Host Game��6"   $
��Left=�66   $��	10,74,0,0q�6Q   $	��Top=�6d   $��96��6|   6�   �� 5   
   -   5         ��Name$ 	lblStatus��5      $�� ��6#   $
��Left=�6.   $��
168,10,0,0q�6I   $	��Top=�6]   $	��182��6u   $��41��6�   $��18(�6�   � 5   
   -   5         �Name$ grdGame��5      $
��Left=�6    $	��306��6;   $��
168,56,0,0q�6H   $	��Top=�6\   $	��306��6t   �� 5   
   -	   5         ��Name$ txtblcStatus��5      $
��Left=�6*   $��
10,272,0,0q�6E   $
��Wrap=�6Y   $	��Top=�6m   $��65��6�   $	��120��6�   $��16(�6�   �� 5   
   -
   5        $ btnReset��5      $��Abort��6#   $
��Left=�63   $��
10,342,0,0q�6N   $	��Top=�6b   $��59��6z   6�   �� 5   
   -   5        $ 
btnRematch��5      $��Rematch��6%   $
��Left=�67   $��
81,342,0,0q�6R   $	��Top=�6f   $��67��6~   {�.  6�   �� 5   
   -   5        $ lblChar��5      $�� ��6!   $
��Left=�6,   $��
355,10,0,0q�6G   $	��Top=�6[   $��41��6s   $	��119��6   $��18(�6�   5      5      