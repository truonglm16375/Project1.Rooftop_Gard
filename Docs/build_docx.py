import os
import zipfile

def build_docx(output_path):
    # XML templates for a clean standard Word document (OpenXML)
    content_types = """<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
  <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
  <Default Extension="xml" ContentType="application/xml"/>
  <Override PartName="/word/document.xml" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml"/>
  <Override PartName="/word/styles.xml" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.styles+xml"/>
</Types>"""

    rels = """<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="word/document.xml"/>
</Relationships>"""

    document_rels = """<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles" Target="styles.xml"/>
</Relationships>"""

    styles = """<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<w:styles xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main">
  <w:docDefaults>
    <w:rPrDefault>
      <w:rPr>
        <w:rFonts w:ascii="Times New Roman" w:hAnsi="Times New Roman" w:cs="Times New Roman"/>
        <w:sz w:val="26"/>
        <w:szCs w:val="26"/>
        <w:lang w:val="vi-VN"/>
      </w:rPr>
    </w:rPrDefault>
    <w:pPrDefault>
      <w:pPr>
        <w:spacing w:line="276" w:lineRule="auto" w:after="120"/>
      </w:pPr>
    </w:pPrDefault>
  </w:docDefaults>
</w:styles>"""

    # Helper functions to build paragraphs
    def escape(t):
        return t.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;").replace('"', "&quot;")

    def p(text="", bold=False, italic=False, size=26, align="left", color="000000", space_after=120, space_before=0):
        jc = f'<w:jc w:val="{align}"/>' if align != "left" else ""
        sp = f'<w:spacing w:before="{space_before}" w:after="{space_after}" w:line="276" w:lineRule="auto"/>'
        rPr = f'<w:rFonts w:ascii="Times New Roman" w:hAnsi="Times New Roman"/><w:sz w:val="{size}"/><w:szCs w:val="{size}"/>'
        if bold: rPr += '<w:b/><w:bCs/>'
        if italic: rPr += '<w:i/><w:iCs/>'
        if color != "000000": rPr += f'<w:color w:val="{color}"/>'
        
        return f'<w:p><w:pPr>{jc}{sp}</w:pPr><w:r><w:rPr>{rPr}</w:rPr><w:t xml:space="preserve">{escape(text)}</w:t></w:r></w:p>'

    def p_code(code_str):
        # Monospace block
        lines = code_str.strip().split("\n")
        xml = ""
        for line in lines:
            xml += f'<w:p><w:pPr><w:spacing w:before="0" w:after="0" w:line="240" w:lineRule="auto"/><w:shd w:val="clear" w:color="auto" w:fill="F2F2F2"/><w:ind w:left="400"/></w:pPr><w:r><w:rPr><w:rFonts w:ascii="Consolas" w:hAnsi="Consolas"/><w:sz w:val="20"/><w:szCs w:val="20"/><w:color w:val="1F3763"/></w:rPr><w:t xml:space="preserve">{escape(line)}</w:t></w:r></w:p>'
        # add a small spacer after code block
        xml += '<w:p><w:pPr><w:spacing w:before="60" w:after="60"/></w:pPr></w:p>'
        return xml

    def p_bullet(title, desc="", size=26, bold_title=True):
        rPr_title = f'<w:rFonts w:ascii="Times New Roman" w:hAnsi="Times New Roman"/><w:sz w:val="{size}"/><w:szCs w:val="{size}"/>'
        if bold_title: rPr_title += '<w:b/><w:bCs/>'
        rPr_desc = f'<w:rFonts w:ascii="Times New Roman" w:hAnsi="Times New Roman"/><w:sz w:val="{size}"/><w:szCs w:val="{size}"/>'
        
        runs = f'<w:r><w:rPr>{rPr_title}</w:rPr><w:t xml:space="preserve">{escape(title)}</w:t></w:r>'
        if desc:
            runs += f'<w:r><w:rPr>{rPr_desc}</w:rPr><w:t xml:space="preserve">{escape(desc)}</w:t></w:r>'
            
        return f'<w:p><w:pPr><w:ind w:left="360" w:hanging="360"/><w:spacing w:before="40" w:after="80" w:line="276" w:lineRule="auto"/></w:pPr><w:r><w:rPr>{rPr_title}</w:rPr><w:t xml:space="preserve">•  </w:t></w:r>{runs}</w:p>'

    body_xml = []
    # Header
    body_xml.append(p("ĐẠI HỌC BÁCH KHOA HÀ NỘI", bold=True, size=28, align="center", space_after=60))
    body_xml.append(p("TRƯỜNG CÔNG NGHỆ THÔNG TIN VÀ TRUYỀN THÔNG", bold=True, size=28, align="center", space_after=140))
    body_xml.append(p("---------------------------------------", align="center", space_after=200))
    
    # Title
    body_xml.append(p("BÁO CÁO TIẾN ĐỘ TUẦN 2", bold=True, size=36, align="center", color="C00000", space_after=200))
    
    # Metadata
    body_xml.append(p("Học phần: Project 1", bold=False, size=26, align="center", space_after=60))
    body_xml.append(p("Giảng viên hướng dẫn: ThS. Nguyễn Hoàng Long", bold=False, size=26, align="center", space_after=60))
    body_xml.append(p("Sinh viên thực hiện: Lê Mạnh Trường", bold=True, size=26, align="center", space_after=60))
    body_xml.append(p("Mã số sinh viên: 202416375", bold=True, size=26, align="center", space_after=300))

    # Section 1
    body_xml.append(p("1. Tên đề tài", bold=True, size=28, space_before=160, space_after=80, color="1F4E79"))
    body_xml.append(p("Xây dựng game 2D mô phỏng trồng cây và nấu ăn: Rooftop Garden (Vườn thượng) trên nền tảng Unity Engine.", space_after=140))

    # Section 2
    body_xml.append(p("2. Mục tiêu của tuần", bold=True, size=28, space_before=160, space_after=80, color="1F4E79"))
    body_xml.append(p_bullet("Tái cấu trúc kiến trúc dữ liệu vật phẩm (Data Architecture): ", "Chuẩn hóa mô hình hướng đối tượng (OOP), phân định rõ ranh giới giữa dữ liệu thực thể cây trồng (PlantData), vật phẩm tổng quát (ItemData), hạt giống (SeedData) và ngăn chứa đồ (ItemStack)."))
    body_xml.append(p_bullet("Phát triển hệ thống kho đồ (Inventory System): ", "Ứng dụng mẫu thiết kế Singleton và phương thức Generic (GetItemByType<T>()) để truy xuất và phân loại vật phẩm an toàn, linh hoạt."))
    body_xml.append(p_bullet("Thiết kế khung giao diện người dùng (UI Framework): ", "Xây dựng UIManager điều phối giao diện theo ngăn xếp (Stack-based navigation) kết hợp lớp cơ sở BaseUIPanel."))
    body_xml.append(p_bullet("Hoàn thiện cơ chế tương tác độc lập giữa Ô đất và Giao diện: ", "Triệt tiêu sự phụ thuộc vòng (decoupling) giữa PlotController và UI thông qua quản lý trung gian PlotManager và mô hình hướng sự kiện."))
    body_xml.append(p_bullet("Kiểm soát chặt chẽ vòng đời sự kiện (Event Lifecycle): ", "Quản lý đóng/ngắt Listener trên các nút bấm động qua OnEnable/OnDisable và hàm khởi tạo SetUp(), bảo đảm an toàn dữ liệu và tối ưu bộ nhớ."))

    # Section 3
    body_xml.append(p("3. Nội dung thực hiện", bold=True, size=28, space_before=160, space_after=80, color="1F4E79"))
    body_xml.append(p("3.1. Thiết kế kiến trúc dữ liệu và Kho đồ (Inventory & Data-Driven OOP)", bold=True, size=26, space_after=60))
    body_xml.append(p_bullet("Xây dựng lớp trừu tượng ItemData: ", "Định nghĩa các trường thuộc tính cơ bản của mọi vật phẩm trong game (itemName, itemIcon, itemType)."))
    body_xml.append(p_bullet("Hiện thực hóa lớp SeedData: ", "Kế thừa từ ItemData, mở rộng các trường đặc thù như giá mua (price), số lượng ban đầu và tham chiếu trực tiếp tới PlantData tương ứng."))
    body_xml.append(p_bullet("Đóng gói ngăn chứa đồ ItemStack: ", "Quản lý cặp dữ liệu ItemData và trường số lượng (amount/stock) trong túi đồ."))
    body_xml.append(p_bullet("Thiết lập hệ thống Inventory: ", "Quản lý danh sách List<ItemStack> dưới mô hình Singleton; tích hợp hàm lọc Generic GetItemByType<T>() cho phép trích xuất nhanh các nhóm dữ liệu mà không cần viết lặp code."))

    body_xml.append(p("3.2. Xây dựng khung điều khiển giao diện (UI Framework)", bold=True, size=26, space_before=100, space_after=60))
    body_xml.append(p_bullet("Xây dựng UIManager theo cơ chế Stack: ", "Quản lý các tầng giao diện dạng ngăn xếp; hỗ trợ phím nóng ESC để tự động đóng lớp panel trên cùng (CloseTopPanel)."))
    body_xml.append(p_bullet("Chuẩn hóa lớp trừu tượng BaseUIPanel: ", "Tái sử dụng mã nguồn cho mọi cửa sổ tương tác (Show/Hide); nghiên cứu và phân định tối ưu giữa cơ chế ẩn (CanvasGroup: alpha, interactable, blocksRaycasts) và cơ chế tắt (GameObject.SetActive)."))
    body_xml.append(p_bullet("Quản lý vòng đời sự kiện nút bấm động: ", "Áp dụng phương pháp làm sạch Listener (btn.onClick.RemoveAllListeners()) trước khi gắn hàm mới trong SetUp() nhằm ngăn chặn rò rỉ bộ nhớ hoặc gọi sự kiện lặp lại."))

    body_xml.append(p("3.3. Tương tác Ô đất và cơ chế Gieo hạt (Plot & UI Decoupling)", bold=True, size=26, space_before=100, space_after=60))
    body_xml.append(p_bullet("Ứng dụng PlotManager làm trung gian: ", "Khi người chơi nhấp vào ô đất, PlotController tự đăng ký thành selectedPlot với PlotManager. Giao diện người dùng chỉ giao tiếp với PlotManager, giữ cho ô đất hoàn toàn độc lập với UI."))
    body_xml.append(p_bullet("Kỹ thuật ép kiểu an toàn: ", "Sử dụng từ khóa is và as trong C# để chuyển đổi an toàn từ ItemStack sang SeedData nhằm lấy PlantData cho phương thức GieoHat()."))

    # Section 4
    body_xml.append(p("4. Kết quả đạt được", bold=True, size=28, space_before=160, space_after=80, color="1F4E79"))
    body_xml.append(p("4.1. Hệ thống dữ liệu hướng đối tượng và Kho đồ", bold=True, size=26, space_after=60))
    body_xml.append(p("Dự án đã phân tách rành mạch cấu trúc dữ liệu theo nguyên lý OOP: ItemData (Abstract) -> SeedData (Inheritance) liên kết với PlantData. Phương thức Generic trong Inventory cho phép truy xuất vật phẩm trực quan và an toàn kiểu dữ liệu:"))
    body_xml.append(p_code("""// Phương thức Generic lọc vật phẩm linh hoạt trong Inventory.cs
public List<T> GetItemByType<T>() where T : ItemData
{
    List<T> result = new List<T>();
    foreach (var stack in item)
    {
        if (stack.data is T typedItem)
        {
            result.Add(typedItem);
        }
    }
    return result;
}"""))

    body_xml.append(p("4.2. Khung giao diện phân tầng và kiểm soát sự kiện", bold=True, size=26, space_before=80, space_after=60))
    body_xml.append(p("Hệ thống UI được chuẩn hóa qua BaseUIPanel và quản lý tập trung bằng UIManager. Việc đăng ký sự kiện cho các nút bấm chọn hạt giống được bảo đảm an toàn qua việc dọn dẹp các Listener cũ:"))
    body_xml.append(p_code("""// Thiết lập nút chọn hạt giống động an toàn trong SeedButtonUI.cs
public void SetUp(SeedData seedData)
{
    btn.onClick.RemoveAllListeners();
    btn.onClick.AddListener(() => {
        PlotManager.Instance.PlantSelectedSeed(seedData.plantData);
        UIManager.Instance.CloseTopPanel();
    });
}"""))

    body_xml.append(p("4.3. Luồng dữ liệu hoàn chỉnh giữa Ô đất và Giao diện", bold=True, size=26, space_before=80, space_after=60))
    body_xml.append(p("Đã hoàn thiện luồng gieo hạt 3 bước khép kín: Người chơi nhấp vào ô đất -> PlotManager ghi nhận selectedPlot -> Bảng chọn hạt giống hiển thị danh sách hạt từ Inventory -> Nhấp chọn hạt, PlotManager điều phối gieo hạt vào ô đất tương ứng và đóng bảng."))

    # Section 5
    body_xml.append(p("5. Minh chứng kết quả (Đề xuất bố trí tư liệu)", bold=True, size=28, space_before=160, space_after=80, color="1F4E79"))
    body_xml.append(p("Các tư liệu minh chứng trực quan được chuẩn bị để tích hợp vào báo cáo bao gồm:"))
    body_xml.append(p_bullet("Hình 1 – Sơ đồ kiến trúc dữ liệu OOP: ", "Biểu đồ thể hiện quan hệ kế thừa ItemData -> SeedData / PlantProductData, liên kết chứa ItemStack và liên kết tham chiếu SeedData.plantData -> PlantData."))
    body_xml.append(p_bullet("Hình 2 – Cấu hình ScriptableObject hạt giống trong Inspector: ", "Ảnh chụp thiết lập thực tế của TomatoSeedData.asset trong Unity Inspector thể hiện rõ tham chiếu tới TomatoPlantData."))
    body_xml.append(p_bullet("Hình 3 – Khởi tạo động bảng chọn hạt giống (SeedSelectionMenu): ", "Ảnh chụp Game View khi nhấn Gieo hạt, hệ thống tự động duyệt kho đồ và sinh các nút hạt giống tương ứng kèm số lượng."))
    body_xml.append(p_bullet("Hình 4 – Cấu trúc ngăn xếp giao diện và đối tượng UIManager: ", "Ảnh chụp cấu trúc Hierarchy và Inspector của UIManager quản lý danh sách các panel."))
    body_xml.append(p_bullet("Hình 5 – Kết quả gieo hạt lên ô đất: ", "Ảnh chụp Game View thể hiện hạt giống chuyển thành Sprite mầm trên mặt ô đất và bảng chọn tự động ẩn đi."))

    # Section 6
    body_xml.append(p("6. Khó khăn và hướng giải quyết", bold=True, size=28, space_before=160, space_after=80, color="1F4E79"))
    body_xml.append(p("6.1. Các vấn đề kỹ thuật đã xử lý thành công", bold=True, size=26, space_after=60))
    body_xml.append(p_bullet("Xung đột mô hình dữ liệu giữa Hạt giống và Cây trồng: ", "Ban đầu sử dụng trực tiếp PlantData cho cả cây và item trong túi đồ, dẫn đến khó khăn khi quản lý số lượng tồn kho và đơn giá. Hướng giải quyết: Tách thành abstract class ItemData, mở rộng SeedData kế thừa ItemData và chứa trường tham chiếu PlantData, đồng thời dùng ItemStack để đóng gói số lượng."))
    body_xml.append(p_bullet("Khó khăn trong luồng giao tiếp giữa Ô đất và UI: ", "Ô đất giữ tham chiếu trực tiếp đến Canvas gây phụ thuộc chặt (tight coupling). Hướng giải quyết: Áp dụng nguyên tắc tách rời: Giao diện gọi hàm của Game Object, Game Object không biết tới Giao diện. Xây dựng PlotManager làm trung gian ghi nhận selectedPlot và điều phối thao tác."))
    body_xml.append(p_bullet("Trùng lặp sự kiện (Double Invoke) khi sinh nút UI động: ", "Khi mở lại bảng chọn hạt giống nhiều lần, sự kiện onClick bị kích hoạt lặp. Hướng giải quyết: Luôn gọi btn.onClick.RemoveAllListeners() trước khi gắn hàm mới trong phương thức SetUp()."))

    body_xml.append(p("6.2. Các vấn đề đang hoàn thiện", bold=True, size=26, space_before=80, space_after=60))
    body_xml.append(p_bullet("Kiểm soát điều kiện tương tác nút bấm: ", "Đang tối ưu logic kiểm tra để tự động vô hiệu hóa các nút Tưới nước/Thu hoạch khi ô đất trống, hoặc nút Gieo hạt khi ô đất đã có cây."))
    body_xml.append(p_bullet("Cơ chế thoát bảng điều khiển khi bấm trượt ra ngoài (Click-outside): ", "Đang tích hợp component PanelClickHandler trên panel nền để tự động ẩn các menu thao tác khi nhấp ra ngoài."))

    # Section 7
    body_xml.append(p("7. Kế hoạch tuần tiếp theo", bold=True, size=28, space_before=160, space_after=80, color="1F4E79"))
    body_xml.append(p_bullet("Hoàn thiện chu kỳ sinh trưởng theo thời gian thực: ", "Kết nối sự kiện GameClock.OnHourPassed để thúc đẩy tiến trình phát triển của cây trồng qua từng giai đoạn (Seed -> Sprout -> Growing -> Mature)."))
    body_xml.append(p_bullet("Hoàn thiện cơ chế Nước tưới và Trạng thái đất: ", "Xây dựng trạng thái đất khô/ướt; chỉ khi đất ẩm cây mới tính giờ tăng trưởng; kích hoạt logic héo úa (Withered) nếu thiếu nước quá thời gian quy định."))
    body_xml.append(p_bullet("Hiện thực hóa chức năng Thu hoạch (Harvesting): ", "Thực thi cộng nông sản thu hoạch vào Inventory, reset ô đất hoặc chuyển sang giai đoạn Harvested cho cây có khả năng tái sinh quả (canRegrow)."))

    # Assemble Document XML
    doc_xml = f"""<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<w:document xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main">
  <w:body>
    {''.join(body_xml)}
    <w:sectPr>
      <w:pgSz w:w="11906" w:h="16838"/>
      <w:pgMar w:top="1440" w:right="1440" w:bottom="1440" w:left="1440"/>
    </w:sectPr>
  </w:body>
</w:document>"""

    # Build ZIP archive (.docx)
    os.makedirs(os.path.dirname(output_path), exist_ok=True)
    with zipfile.ZipFile(output_path, 'w', zipfile.ZIP_DEFLATED) as z:
        z.writestr("[Content_Types].xml", content_types)
        z.writestr("_rels/.rels", rels)
        z.writestr("word/_rels/document.xml.rels", document_rels)
        z.writestr("word/document.xml", doc_xml)
        z.writestr("word/styles.xml", styles)

    print("Success: Generated DOCX at", output_path)

if __name__ == "__main__":
    out_file = r"e:\Unity st\Projects\Rooftop Gard\Docs\BAO_CAO_TIEN_DO_TUAN_2.docx"
    build_docx(out_file)
