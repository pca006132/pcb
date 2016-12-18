using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls.Dialogs;
using pcb.core.chain;

namespace pcb
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings
    {
        MainWindow parent;
        public Settings(MainWindow window)
        {
            InitializeComponent();
            parent = window;

            FontFamily setting_font = new FontFamily(Properties.UIresources.font);
            FontFamily = setting_font;

            foreach (FontFamily font in FontList.Items)
            {
                if (font == parent.Editor.FontFamily)
                {
                    FontList.SelectedItem = font;
                }
            }
            if (FontList.SelectedIndex < 0)
            {
                if (FontList.Items.Contains(new FontFamily("Microsoft Yahei")))
                    FontList.SelectedItem = new FontFamily("Microsoft Yahei");
                else FontList.SelectedIndex = 0;
            }

            UseAutocomplete.IsChecked = parent.useAutocomplete;
            UseAECAsMarker.IsChecked = !core.PcbParser.markerType;
            showSpace.IsChecked = parent.Editor.Options.ShowSpaces;
            placeDir.SelectedIndex = StraightCbChain.initialDir == core.util.Direction.positiveY ? 1 : (StraightCbChain.initialDir == core.util.Direction.positiveX) ? 0 : 2;
            CBCount.Text = StraightCbChain.limit.ToString();
            UseBox.IsChecked = parent.useBlockStruc;
            block_X.Text = BoxCbChain.xLimit.ToString();
            block_z.Text = BoxCbChain.zLimit.ToString();
            blockID_Top.Text = BoxCbChain.baseBlock;
            blockID_Side.Text = BoxCbChain.outerBlock;
            blockDamage_Top.Text = BoxCbChain.baseDamage.ToString();
            blockDamage_Side.Text = BoxCbChain.outerDamage.ToString();
            limit.Text = core.SingleOOC.oocLimit.ToString();
            version.IsChecked = core.PcbParser.version_1_11;
            Show();
        }        
        private void cancel(object sender, EventArgs e)
        {
            this.Close();
        }
        private void ApplyChange(object sender, EventArgs e)
        {
            parent.useAutocomplete = UseAutocomplete.IsChecked == true;
            core.PcbParser.markerType = UseAECAsMarker.IsChecked == false;
            parent.Editor.Options.ShowSpaces = showSpace.IsChecked == true;
            parent.useBlockStruc = UseBox.IsChecked == true;
            BoxCbChain.baseBlock = blockID_Top.Text;
            BoxCbChain.outerBlock = blockID_Side.Text;

            if (placeDir.SelectedIndex == 0)
                StraightCbChain.initialDir = core.util.Direction.positiveX;
            else if (placeDir.SelectedIndex == 1)            
                StraightCbChain.initialDir = core.util.Direction.positiveY;
            else            
                StraightCbChain.initialDir = core.util.Direction.positiveZ;
            
            parent.FontFamily = (FontFamily)FontList.SelectedItem;
            int temp;
            bool isChinese = System.Threading.Thread.CurrentThread.CurrentCulture.Name == "zh";
            if (int.TryParse(block_X.Text, out temp) == false || temp < 2)
            {
                this.ShowMessageAsync(Properties.Resources.error,  isChinese? "X轴长度请填写大于2的整数" : "X axis length must be an integer larger than 2");
                return;
            }
            BoxCbChain.xLimit = temp;
            if (int.TryParse(block_z.Text, out temp) == false || temp < 2)
            {
                this.ShowMessageAsync(Properties.Resources.error, isChinese ? "Z轴长度请填写大于2的整数" : "Z axis length must be an integer larger than 2");
                return;
            }
            BoxCbChain.zLimit = temp;
            if (int.TryParse(blockDamage_Top.Text, out temp) == false || temp < 0 || temp > 16)
            {
                this.ShowMessageAsync(Properties.Resources.error, isChinese? "数据值请填写大于0，小于16的整数" : "data value should be integer >= 0 and <= 15");
                return;
            }
            BoxCbChain.baseDamage = (byte)temp;
            if (int.TryParse(blockDamage_Side.Text, out temp) == false || temp < 0 || temp > 16)
            {
                this.ShowMessageAsync(Properties.Resources.error, isChinese ? "数据值请填写大于0，小于16的整数" : "data value should be integer >= 0 and <= 15");
                return;
            }
            BoxCbChain.outerDamage = (byte)temp;
            if (int.TryParse(CBCount.Text, out temp) == false || (temp < 2 && temp != 0))
            {
                this.ShowMessageAsync(Properties.Resources.error, isChinese ? "自动换行CB数必须为大于等于2的整数" : "CB count should be integer larger than or equal to 2, or 0");
                return;
            }
            StraightCbChain.limit = temp;
            if (!int.TryParse(limit.Text, out temp) || temp < 1000)
            {
                this.ShowMessageAsync(Properties.Resources.error, isChinese ? "OOC字符数必须为大于等于1000的整数" : "OOC char limit should be an integer not smaller than 1000");
                return;
            }
            core.SingleOOC.oocLimit = temp;
            core.PcbParser.version_1_11 = version.IsChecked == true;
            Close();
        }
    }
}
