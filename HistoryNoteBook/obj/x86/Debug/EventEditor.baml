   M S B A M L   `   `   `  ����  HistoryNoteBook!    HistoryNoteBook.EventEditor   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  EventEditor��5   	   $	��300��6   $	��476��6*    � 
ResizeMode$ NoResize=�66   .��5      �  � ColumnDefinitions 5   
   �� 5      $
��287* �6   �� 5      $
��167* �6    � RowDefinitions 5
   
   �� 5      $
��180* �6   �� 5      $	��37* �6   �� 5      $	��44* �6   .+�5   
   �� -   5        $��textBox_Content��5   M   $	��148��6   $
��Left=�6   $��	26,19,0,0q�6:   $	��Top=�6d   $	��221��6|    �� TextWrapping$
 Wrap=�6�   �� 5   
   -   5        $��	button_OK��5   W   $��确定��6   $��23��6   $
��Left=�6*   $��86,9,0,0q�6E   $	��Top=�6h   $��75��6�   $��2��6�   6�   �� 5   
   -   5        $��button_Cancel��5   X   $��取消��6   $��23��6   $��Right=�6*   $��0,9,22,0q�6F   $	��Top=�6m   $��75��6�   $��2��6�   6�   �� 5   
   -   5        $��stackPanel1��5   J   $��1��6   $��37��6"   $��Right=�6.   $��Center=�6]   $	��278��6x    �� Orientation$ 
Horizontal=�6�   $��RightToLeft=�6�   .+�5      �� -   5        $��label3��5   ,   $	��日��6   $��28��6    6,   �� 5      -   5        $��textBox_day��5   "   $��23��6   $��42��65   $��Right=�6@   $��LeftToRight=�6c   �� 5      -   5        $��label2��5   ,   $	��月��6   $��28��6    6,   �� 5      -   5        $��textBox_month��5   "   $��23��6   $��42��67   $��LeftToRight=�6b   $��Right=�6~   �� 5      -	   5        $��label1��5   ,   $	��年��6   $��28��6    6,   �� 5      -
   5        $��textBox_year��5   "   $��23��6   $��42��66   $��Right=�6`   $��LeftToRight=�6�   5      �� 5   
   -   5        $��listBox_Tag��5   ]   $��1��6   $	��148��6"   $
��Left=�6/   $��	36,19,0,0q�6J   $	��Top=�6p   $	��120��6�   �� 5   
   -   5        $��button_AddTag��5   v   $��添加标签��6   $��1��6    $��1��60   $��23��6=   $
��Left=�6I   $��65,7,0,0q�6d   $	��Top=�6�   $��75��6�   6�   5      5      